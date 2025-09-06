using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
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
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> _repository;

        public CountryService(IRepository<Country> countryRepository)
        {
            _repository = countryRepository;
        }
        public List<Country> GetAll()
        {
             return _repository.GetAll(selector: c => c).ToList();
        }

        public async Task<IPagedList<Country>> GetAllCountriesPagedAndSortedAsync(string sortOrder, string searchString, int pageNumber, int pageSize)
        {
            var countriesQuery = _repository.GetAllAsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                countriesQuery = countriesQuery.Where(c => c.Name.Contains(searchString)
                                                         || c.Capital.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    countriesQuery = countriesQuery.OrderByDescending(c => c.Name);
                    break;
                case "capital":
                    countriesQuery = countriesQuery.OrderBy(c => c.Capital);
                    break;
                case "capital_desc":
                    countriesQuery = countriesQuery.OrderByDescending(c => c.Capital);
                    break;
                default:
                    countriesQuery = countriesQuery.OrderBy(c => c.Name);
                    break;
            }

            var totalItemCount = await countriesQuery.CountAsync();

            var itemsForCurrentPage = await countriesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Country>(itemsForCurrentPage, pageNumber, pageSize, totalItemCount);
        }

        public Country? GetById(Guid id)
        {
            return _repository.Get(selector: c => c,
                                      predicate: c => c.Id == id);
        }

        public Country Insert(Country country)
        {
            country.Id = Guid.NewGuid();
            return _repository.Insert(country);
        }
        public ICollection<Country> InsertMany(ICollection<Country> countries)
        {
            return _repository.InsertMany(countries);
        }

        public Country Update(Country country)
        {
            return _repository.Update(country);
        }

        public Country DeleteById(Guid id)
        {
            var country = GetById(id) ?? throw new Exception("Country not found");
            return _repository.Delete(country);
        }
    }
}
