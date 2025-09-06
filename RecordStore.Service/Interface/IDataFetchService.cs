using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Interface
{
    public interface IDataFetchService
    {
        Task<List<Country>> GetAllCountriesAsync();
        Task<List<Country>> ImportTenNewCountriesAsync();
        Task<List<Artist>> ImportArtistsAsync();
    }
}