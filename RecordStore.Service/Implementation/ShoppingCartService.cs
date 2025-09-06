using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using RecordStore.Domain.DTO.Email;
using RecordStore.Domain.Identity;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<RecordInShoppingCart> _recordInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Record> _recordRepository;
        private readonly IEmailService _emailService;

        public ShoppingCartService
        (
            IRepository<ShoppingCart> shoppingCartRepository,
            IRepository<RecordInShoppingCart> recordInShoppingCartRepository,
            IUserRepository userRepository,
            IRepository<Order> orderRepository,
            IRepository<Record> recordRepository,
            IEmailService emailService
        )
        {
            _shoppingCartRepository = shoppingCartRepository;
            _recordInShoppingCartRepository = recordInShoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _recordRepository = recordRepository;
            _emailService = emailService;
        }

        public ShoppingCart? GetByUserId(Guid userId)
            => _shoppingCartRepository.Get(
                    selector: x => x,
                    predicate: x => x.OwnerId.Equals(userId.ToString())
               );

        public ShoppingCartDTO GetByUserIdWithIncludedPrducts(Guid userId, bool applyPoints = false)
        {
            var cart = _shoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.OwnerId == userId.ToString(),
                include: q => q.Include(z => z.AllRecords)
                               .ThenInclude(r => r.Record)
            );

            if (cart == null)
                throw new Exception("Shopping-cart not found");

            var user = _userRepository.GetUserById(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var items = cart.AllRecords.Select(r => new AddToCartDTO
            {
                RecordId = r.RecordId,
                Title = r.Record?.Title,
                CoverURL = r.Record?.CoverURL,
                Price = r.Record?.Price,
                Quantity = r.Quantity
            })
            .ToList();

            return new ShoppingCartDTO
            {
                ShoppingCartId = cart.Id,
                Items = items,
                UserLoyaltyPoints = user.LoyaltyPoints,
                PointsApplied = applyPoints
            };
        }

        public void IncreaseQuantityInShoppingCart(Guid userId, Guid recordId)
        {
            var userShoppingCart = GetByUserId(userId);
            if (userShoppingCart == null) throw new Exception("Shopping cart not found for this user.");

            var itemInCart = _recordInShoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.ShoppingCartId == userShoppingCart.Id && x.RecordId == recordId
            );

            if (itemInCart == null) throw new Exception("Record in cart not found.");

            var record = _recordRepository.Get(selector: x => x, predicate: x => x.Id == recordId);
            if (record == null) throw new Exception("Record not found in the store.");

            if (itemInCart.Quantity < record.StockQuantity)
            {
                itemInCart.Quantity++;
                _recordInShoppingCartRepository.Update(itemInCart);
            }
            else
            {
                throw new Exception($"Cannot add another '{record.Title}'. The maximum stock of {record.StockQuantity} has been reached.");
            }
        }

        public void ReduceQuantityInShoppingCart(Guid userId, Guid recordId)
        {
            var userShoppingCart = GetByUserId(userId);
            if (userShoppingCart == null) throw new Exception("Shopping cart not found for this user.");

            var itemToReduce = _recordInShoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.ShoppingCartId == userShoppingCart.Id && x.RecordId == recordId
            );

            if (itemToReduce == null) throw new Exception("Record in cart not found.");

            if (itemToReduce.Quantity > 1)
            {
                itemToReduce.Quantity--;
                _recordInShoppingCartRepository.Update(itemToReduce);
            }
            else
            {
                _recordInShoppingCartRepository.Delete(itemToReduce);
            }
        }

        public void DeleteProductFromShoppingCart(Guid userId, Guid recordId)
        {
            var userShoppingCart = GetByUserId(userId);
            if (userShoppingCart == null) throw new Exception("Shopping cart not found for this user.");

            var itemToDelete = _recordInShoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.ShoppingCartId == userShoppingCart.Id && x.RecordId == recordId
            );

            if (itemToDelete == null) throw new Exception("Record in cart not found.");
            _recordInShoppingCartRepository.Delete(itemToDelete);
        }

        public async Task<(bool IsSuccess, string EmailContent)> OrderRecords(Guid userId, bool pointsApplied)
        {
            var userCart = _shoppingCartRepository.Get(
                selector: x => x,
                predicate: x => x.OwnerId.Equals(userId.ToString()),
                include: q => q.Include(z => z.AllRecords).ThenInclude(r => r.Record)
            );

            var loggedInUser = _userRepository.GetUserById(userId.ToString());
            if (loggedInUser == null) throw new Exception("Logged in user not found.");

            if (userCart == null || !userCart.AllRecords.Any()) return (true, "");

            decimal originalTotalPrice = userCart.AllRecords.Sum(ris => (ris.Record?.Price ?? 0) * ris.Quantity);
            decimal discountApplied = 0;

            if (pointsApplied)
            {
                discountApplied = Math.Min(originalTotalPrice, loggedInUser.LoyaltyPoints);
                loggedInUser.LoyaltyPoints -= discountApplied;
            }

            decimal loyaltyPointsEarned = originalTotalPrice * 0.05m;
            loggedInUser.LoyaltyPoints += loyaltyPointsEarned;

            _userRepository.UpdateUser(loggedInUser);

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                OwnerId = loggedInUser.Id,
                Owner = loggedInUser,
                RecordsInOrder = userCart.AllRecords.Select(item => new RecordInOrder
                {
                    RecordId = item.RecordId,
                    Quantity = item.Quantity
                }).ToList()
            };

            _orderRepository.Insert(newOrder);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<h2>Your Order Confirmation</h2>");
            sb.AppendLine($"<p>Dear {loggedInUser.UserName},</p>");
            sb.AppendLine("<p>Thank you for your order! Your order has been placed successfully.</p>");
            sb.AppendLine($"<h3>Order Details (Order ID: {newOrder.Id.ToString().Substring(0, 25)}...)</h3>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='width:100%; border-collapse:collapse;'>");
            sb.AppendLine("<thead><tr><th>#</th><th>Record Title</th><th>Quantity</th><th>Price per Item</th><th>Subtotal</th></tr></thead>");
            sb.AppendLine("<tbody>");

            int itemCounter = 1;
            foreach (var item in userCart.AllRecords)
            {
                decimal itemPrice = item.Record?.Price ?? 0;
                decimal subtotal = itemPrice * item.Quantity;
                sb.AppendLine($"<tr>");
                sb.AppendLine($"<td>{itemCounter++}</td>");
                sb.AppendLine($"<td>{item.Record?.Title ?? "N/A"}</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>{itemPrice:C}</td>");
                sb.AppendLine($"<td>{subtotal:C}</td>");
                sb.AppendLine($"</tr>");

                if (item.Record != null && item.Record.StockQuantity.HasValue)
                {
                    item.Record.StockQuantity -= item.Quantity;
                    if (item.Record.StockQuantity < 0)
                    {
                        item.Record.StockQuantity = 0;
                    }
                    _recordRepository.Update(item.Record);
                }
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("<tfoot>");
            sb.AppendLine($"<tr><td colspan='4' align='right'><strong>Original Total:</strong></td><td><strong>{originalTotalPrice:C}</strong></td></tr>");
            if (discountApplied > 0)
            {
                sb.AppendLine($"<tr><td colspan='4' align='right'>Discount Applied:</td><td>-{discountApplied:C}</td></tr>");
                sb.AppendLine($"<tr><td colspan='4' align='right'><strong>Final Price:</strong></td><td><strong>{(originalTotalPrice - discountApplied):C}</strong></td></tr>");
            }
            sb.AppendLine("</tfoot></table>");

            sb.AppendLine($"<p>You have earned {loyaltyPointsEarned:0.00} loyalty points from this purchase.</p>");
            sb.AppendLine("<p>Best regards,<br/>The Record Store Team</p>");

            var emailMessage = new EmailMessage
            {
                MailTo = loggedInUser.Email,
                Subject = "Record Store: Your Order #" + newOrder.Id.ToString().Substring(0, 8) + "... Confirmed!",
                Content = sb.ToString()
            };

            await _emailService.SendEmailAsync(emailMessage);

            foreach (var recordInCart in userCart.AllRecords.ToList())
            {
                _recordInShoppingCartRepository.Delete(recordInCart);
            }

            return (true, sb.ToString());
        }
    }
}