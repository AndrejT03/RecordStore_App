using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Service.Implementation
{
    public class ArtistService : IArtistService
    {
        private readonly IRepository<Artist> _repository;
        public ArtistService(IRepository<Artist> ArtistRepository)
        {
            _repository = ArtistRepository;
        }
        public List<Artist> GetAll()
        {
            return _repository.GetAll(selector: a => a).ToList();
        }

        public Artist? GetById(Guid id)
        {
            return _repository.Get(
                selector: a => a,
                predicate: a => a.Id == id,
                include: a => a
                    .Include(x => x.Country)
                    .Include(x => x.Records)
            );
        }

        public Artist Insert(Artist artist)
        {
            artist.Id = Guid.NewGuid();
            return _repository.Insert(artist);
        }
        public void InsertMany(List<Artist> artists)
        {
            _repository.InsertMany(artists);
        }
        public Artist Update(Artist artist)
        {
            return _repository.Update(artist);
        }
        public Artist DeleteById(Guid id)
        {
            var artist = GetById(id) ?? throw new Exception("Artist not found");
            return _repository.Delete(artist);
        }
        public async Task<IPagedList<Artist>> GetAllPagedAndSortedAsync(string sortOrder, string searchString, int pageNumber, int pageSize)
        {
            var artistsQuery = _repository.GetAllAsQueryable(
                include: q => q.Include(a => a.Country)
            ).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                artistsQuery = artistsQuery.Where(a => a.Name.Contains(searchString) || a.Country.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    artistsQuery = artistsQuery.OrderByDescending(a => a.Name);
                    break;
                case "country":
                    artistsQuery = artistsQuery.OrderBy(a => a.Country.Name);
                    break;
                case "country_desc":
                    artistsQuery = artistsQuery.OrderByDescending(a => a.Country.Name);
                    break;
                default:
                    artistsQuery = artistsQuery.OrderBy(a => a.Name);
                    break;
            }

            var totalItemCount = await artistsQuery.CountAsync();
            var itemsForCurrentPage = await artistsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Artist>(itemsForCurrentPage, pageNumber, pageSize, totalItemCount);
        }
    }
}
