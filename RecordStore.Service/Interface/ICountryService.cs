using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Service.Interface
{
    public interface ICountryService
    {
        List<Country> GetAll();
        Task<IPagedList<Country>> GetAllCountriesPagedAndSortedAsync(string sortOrder, string searchString, int pageNumber, int pageSize);
        Country? GetById(Guid id);
        Country Insert(Country country);
        ICollection<Country> InsertMany(ICollection<Country> countries);
        Country Update(Country country);
        Country DeleteById(Guid id);
    }
}