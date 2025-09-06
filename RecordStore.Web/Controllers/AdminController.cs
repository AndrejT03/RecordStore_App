using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using RecordStore.Domain.DTO.Analytics;
using RecordStore.Repository.Interface;
using RecordStore.Service.Implementation;
using RecordStore.Service.Interface;
using System.IO;
using System.Linq;
using System.Globalization;

namespace RecordStore.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IDataFetchService _fetchService;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRecordService _recordService;

        public AdminController(IUserRepository userRepository, IRepository<Order> orderRepository, IDataFetchService fetchService, IRepository<Review> reviewRepository, IRecordService recordService)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _fetchService = fetchService;
            _reviewRepository = reviewRepository;
            _recordService = recordService;
        }

        public IActionResult Dashboard()
        {
            var users = _userRepository.GetAllUsers().ToList();
            var orders = _orderRepository.GetAll()
               .Include(o => o.Owner) 
               .Include(o => o.RecordsInOrder) 
               .ThenInclude(rio => rio.Record) 
               .ToList();
            var records = _recordService.GetAll()
                .ToList();
            var reviews = _reviewRepository.GetAll()
                .Include(r => r.User)
                .Include(r => r.Record)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalUsers = users.Count,
                TotalOrders = orders.Count,
                TotalRecords = records.Count,
                TotalReviews = reviews.Count,
                AllUsers = users,
                AllOrders = orders,
                AllRecords = records,
                AllReviews = reviews
            };
            return View(viewModel);
        }
        public IActionResult Analytics()
        {
            var orders = _orderRepository.GetAll()
                .Include(o => o.RecordsInOrder)
                .ThenInclude(rio => rio.Record)
                .ToList();

            var salesOverTime = orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(o => o.RecordsInOrder.Sum(i => i.Quantity * (i.Record?.Price ?? 0)))
                })
                .OrderBy(s => s.Date)
                .ToList();

            var salesLabels = salesOverTime.Select(s => s.Date.ToString("MMM dd"));
            var salesData = salesOverTime.Select(s => s.Total);

            var bestSellingRecords = orders
                .SelectMany(o => o.RecordsInOrder)
                .GroupBy(rio => rio.Record)
                .Select(g => new BestSellingRecordViewModel
                {
                    RecordId = g.Key.Id,
                    Title = g.Key.Title,
                    ArtistName = g.Key.Artist?.Name,
                    UnitsSold = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(r => r.UnitsSold)
                .Take(10)
                .ToList();

            var genrePerformance = orders
                .SelectMany(o => o.RecordsInOrder)
                .GroupBy(rio => rio.Record.Genre)
                .Select(g => new GenrePerformanceViewModel
                {
                    Genre = g.Key.ToString(),
                    TotalRevenue = g.Sum(i => i.Quantity * (i.Record?.Price ?? 0))
                })
                .OrderByDescending(gp => gp.TotalRevenue)
                .ToList();

            var viewModel = new AnalyticsViewModel
            {
                SalesChartLabels = salesLabels,
                SalesChartData = salesData,
                BestSellingRecords = bestSellingRecords,
                GenrePerformance = genrePerformance
            };

            return View(viewModel);
        }

        public IActionResult ExportUsers()
        {
            string fileName = "ExportedUsers.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Users");

                worksheet.Cell(1, 1).Value = "User ID";
                worksheet.Cell(1, 2).Value = "First Name";
                worksheet.Cell(1, 3).Value = "Last Name";
                worksheet.Cell(1, 4).Value = "Email";
                worksheet.Cell(1, 5).Value = "Address";
                worksheet.Cell(1, 6).Value = "Date Created";

                var users = _userRepository.GetAllUsers().ToList();

                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    var currentRow = i + 2; 

                    worksheet.Cell(currentRow, 1).Value = user.Id;
                    worksheet.Cell(currentRow, 2).Value = user.FirstName;
                    worksheet.Cell(currentRow, 3).Value = user.LastName;
                    worksheet.Cell(currentRow, 4).Value = user.Email;
                    worksheet.Cell(currentRow, 5).Value = user.Address;
                    worksheet.Cell(currentRow, 6).Value = user.CreatedAt.ToString("yyyy-MM-dd");
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }
        }

        public IActionResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Customer Email";
                worksheet.Cell(1, 3).Value = "Record Title";
                worksheet.Cell(1, 4).Value = "Quantity";
                worksheet.Cell(1, 5).Value = "Price Per Item";

                var allOrders = _orderRepository.GetAll()
                    .Include(o => o.Owner)
                    .Include(o => o.RecordsInOrder)
                    .ThenInclude(rio => rio.Record) 
                    .ToList();

                var currentRow = 2;

                foreach (var order in allOrders)
                {
                    foreach (var item in order.RecordsInOrder)
                    {
                        worksheet.Cell(currentRow, 1).Value = order.Id.ToString();
                        worksheet.Cell(currentRow, 2).Value = order.Owner?.Email;
                        worksheet.Cell(currentRow, 3).Value = item.Record?.Title; 
                        worksheet.Cell(currentRow, 4).Value = item.Quantity;
                        worksheet.Cell(currentRow, 5).Value = item.Record?.Price;
                        currentRow++;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportTen()
        {
            var importedCountries = await _fetchService.ImportTenNewCountriesAsync();
            TempData["ImportMessage"] = $"Успешно импортирани {importedCountries.Count} нови земји.";
            return RedirectToAction("Index", "Countries"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportDynamicArtists()
        {
            try
            {
                var importedArtists = await _fetchService.ImportArtistsAsync();

                if (importedArtists.Any())
                {
                    TempData["SuccessMessage"] = $"Successfully imported {importedArtists.Count} new artist(s).";
                }
                else
                {
                    TempData["InfoMessage"] = "No new artists were imported. They might already exist, their country is not in the database, or the random search was unlucky. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during the import process. Please check the logs.";
            }

            return RedirectToAction("Index", "Artists");
        }

        public IActionResult ManageReviews()
        {
            var allReviews = _reviewRepository.GetAll()
                .Include(r => r.Record)
                .Include(r => r.User)
                .ToList();

            var groupedReviews = allReviews
                .Where(r => r.Record != null) 
                .GroupBy(r => r.Record)
                .OrderBy(g => g.Key.Title) 
                .ToList();

            return View(groupedReviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteReview(Guid id)
        {
            var review = _reviewRepository.GetAll().FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                TempData["ErrorReviewMessage"] = "Review not found.";
                return RedirectToAction("ManageReviews");
            }

            _reviewRepository.Delete(review);

            TempData["SuccessReviewMessage"] = "Review successfully deleted.";
            return RedirectToAction("ManageReviews");
        }

        public IActionResult ManageInventory()
        {
            var allRecords = _recordService.GetAll();
            return View(allRecords);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(Guid RecordId, int NewQuantity)
        {
            var success = await _recordService.UpdateStock(RecordId, NewQuantity);

            if (success)
            {
                TempData["SuccessInventoryMessage"] = "Stock updated successfully.";
            }
            else
            {
                TempData["ErrorInventoryMessage"] = "Failed to update stock. Record not found or quantity was invalid.";
            }

            return RedirectToAction(nameof(ManageInventory));
        }
    }
}