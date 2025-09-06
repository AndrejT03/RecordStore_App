using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Service.Interface
{
    public interface IRecordService
    {
        List<Record> GetAll();
        Record? GetById(Guid id);
        Record Insert(Record record);
        Record Update(Record record);
        Record DeleteById(Guid id);
        AddToCartDTO GetSelectedShoppingCartProduct(Guid id);
        void AddProductToSoppingCart(Guid recordId, Guid userId, int quantity);
        Task<bool> UpdateStock(Guid recordId, int newQuantity);
        void AddTracksToRecord(AddTracklistDTO dto);
        void DeleteTrack(Guid trackId);
        Task<IPagedList<Record>> GetAllPagedAndSortedAsync(string sortOrder, string searchString, Genre? filterGenre, RecordFormat? filterFormat, bool? filterIsReissue, int pageNumber, int pageSize);
    }
}