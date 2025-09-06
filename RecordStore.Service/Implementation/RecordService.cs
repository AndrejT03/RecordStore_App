using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using RecordStore.Repository.Implementation;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Service.Implementation
{
    public class RecordService : IRecordService
    {
        private readonly IRepository<Record> _recordRepository;
        private readonly IRepository<RecordInShoppingCart> _recordInCartRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRepository<Track> _trackRepository;

        public RecordService(IRepository<Record> recordRepository,
                         IRepository<RecordInShoppingCart> recordInCartRepository,
                         IShoppingCartService shoppingCartService,
                         IRepository<Track> trackRepository)
        {
            _recordRepository = recordRepository;
            _recordInCartRepository = recordInCartRepository;
            _shoppingCartService = shoppingCartService;
            _trackRepository = trackRepository;
        }

        public List<Record> GetAll()
        {
            return _recordRepository.GetAll(selector: r => r,
                                            include: r => r
                                            .Include(x => x.Artist)
                                            .Include(r => r.RecordLabel)).ToList();
        }

        public Record? GetById(Guid id)
        {
            return _recordRepository.Get(
                selector: r => r,
                predicate: r => r.Id == id,
                include: r => r
                    .Include(x => x.Artist)
                    .ThenInclude(y => y.Country)
                    .Include(x => x.Reviews)
                    .ThenInclude(y => y.User)
                    .Include(x => x.RecordLabel)
                    .ThenInclude(y => y.Country)
                    .Include(r => r.Tracks)
            );
        }
        public Record Insert(Record record)
        {
            record.Id = Guid.NewGuid();
            return _recordRepository.Insert(record);
        }

        public Record Update(Record record)
        {
            return _recordRepository.Update(record);
        }

        public Record DeleteById(Guid id)
        {
            var record = GetById(id) ?? throw new Exception("Record not found");
            return _recordRepository.Delete(record);
        }

        public AddToCartDTO GetSelectedShoppingCartProduct(Guid id)
        {
            var rec = GetById(id) ?? throw new Exception("Record not found");

            return new AddToCartDTO
            {
                RecordId = rec.Id,
                Title = rec.Title,
                CoverURL = rec.CoverURL,
                Price = rec.Price,
                Quantity = 1
            };
        }

        public void AddProductToSoppingCart(Guid recordId, Guid userId, int quantity)
        {
            var cart = _shoppingCartService.GetByUserId(userId)
                   ?? throw new Exception("Shopping cart not found");

            var record = GetById(recordId)
                         ?? throw new Exception("Record not found");

            UpdateCartItem(record, cart, quantity);
        }

        private void UpdateCartItem(Record record, ShoppingCart cart, int quantity)
        {
            var existingItem = GetRecordInCart(record.Id, cart.Id);

            if (existingItem == null)
            {
                var newItem = new RecordInShoppingCart
                {
                    Id = Guid.NewGuid(),
                    RecordId = record.Id,
                    ShoppingCartId = cart.Id,
                    Record = record,
                    ShoppingCart = cart,
                    Quantity = quantity
                };

                _recordInCartRepository.Insert(newItem);
            }
            else
            {
                existingItem.Quantity += quantity;
                _recordInCartRepository.Update(existingItem);
            }
        }

        public async Task<bool> UpdateStock(Guid recordId, int newQuantity)
        {
            if (newQuantity < 0)
            {
                return false; 
            }

            var record = GetById(recordId);

            if (record == null)
            {
                return false; 
            }

            record.StockQuantity = newQuantity;
            _recordRepository.Update(record);
            return true;
        }

        private RecordInShoppingCart? GetRecordInCart(Guid recordId, Guid cartId)
        {
               return _recordInCartRepository.Get(selector: x => x,
                                                predicate: x => x.RecordId == recordId && x.ShoppingCartId == cartId);
        }
        public void AddTracksToRecord(AddTracklistDTO dto)
        {
            var record = GetById(dto.RecordId);
            if (record == null)
            {
                throw new Exception("Record not found.");
            }

            foreach (var existingTrack in record.Tracks.ToList())
            {
                _trackRepository.Delete(existingTrack);
            }

            int currentTrackNumber = 1;
            if (dto.Tracks != null)
            {
                foreach (var track in dto.Tracks)
                {
                    if (!string.IsNullOrWhiteSpace(track.Title))
                    {
                        track.Id = Guid.NewGuid();
                        track.RecordId = dto.RecordId;
                        track.TrackNumber = currentTrackNumber++; 
                        _trackRepository.Insert(track);
                    }
                }
            }
        }

        public void DeleteTrack(Guid trackId)
        {
            var track = _trackRepository.GetAll().FirstOrDefault(t => t.Id == trackId);
            if (track != null)
            {
                _trackRepository.Delete(track);
            }
            else
            {
                throw new Exception("Track not found.");
            }
        }

        public async Task<IPagedList<Record>> GetAllPagedAndSortedAsync(string sortOrder,
                                                                        string searchString,
                                                                        Genre? filterGenre,
                                                                        RecordFormat? filterFormat,
                                                                        bool? filterIsReissue,
                                                                        int pageNumber,
                                                                        int pageSize)
        {
            var recordsQuery = _recordRepository.GetAllAsQueryable(
                include: q => q.Include(r => r.Artist)
                                .Include(r => r.RecordLabel)
            ).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                recordsQuery = recordsQuery.Where(r =>
                    r.Title.Contains(searchString) ||
                    r.Artist.Name.Contains(searchString) ||
                   (r.RecordLabel != null && r.RecordLabel.Name.Contains(searchString)));
            }

            if (filterGenre.HasValue)
            {
                recordsQuery = recordsQuery.Where(r => r.Genre == filterGenre);
            }

            if (filterFormat.HasValue)
            {
                recordsQuery = recordsQuery.Where(r => r.Format == filterFormat);
            }

            if (filterIsReissue.HasValue)
            {
                recordsQuery = recordsQuery.Where(r => r.IsReissue == filterIsReissue);
            }

            switch (sortOrder)
            {
                case "title_desc":
                    recordsQuery = recordsQuery.OrderByDescending(r => r.Title);
                    break;
                case "Price":
                    recordsQuery = recordsQuery.OrderBy(r => r.Price);
                    break;
                case "price_desc":
                    recordsQuery = recordsQuery.OrderByDescending(r => r.Price);
                    break;
                case "date_desc":
                    recordsQuery = recordsQuery.OrderByDescending(r => r.ReleaseYear);
                    break;
                case "Date":
                    recordsQuery = recordsQuery.OrderBy(r => r.ReleaseYear);
                    break;
                default: 
                    recordsQuery = recordsQuery.OrderBy(r => r.Title);
                    break;
            }

            var totalItemCount = await recordsQuery.CountAsync();
            var itemsForCurrentPage = await recordsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Record>(itemsForCurrentPage, pageNumber, pageSize, totalItemCount);
        }
    }
}