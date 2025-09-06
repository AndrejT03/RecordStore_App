using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Service.Interface
{
    public interface IArtistService
    {
        List<Artist> GetAll();
        Artist? GetById(Guid id);
        Artist Insert(Artist artist);
        void InsertMany(List<Artist> artists);
        Artist Update(Artist artist);
        Artist DeleteById(Guid id);
        Task<IPagedList<Artist>> GetAllPagedAndSortedAsync(string sortOrder, string searchString, int pageNumber, int pageSize);
    }
}