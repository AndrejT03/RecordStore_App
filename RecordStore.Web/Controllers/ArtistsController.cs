using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace RecordStore.Web.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly ICountryService _countryService;

        public ArtistsController(IArtistService service, ICountryService countryService)
        {
            _artistService = service;
            _countryService = countryService;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var artists = await _artistService.GetAllPagedAndSortedAsync(sortOrder, searchString, pageNumber, pageSize);

            return View(artists);
        }

        public IActionResult Details(Guid? id)
        {
            var artist = _artistService.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_countryService.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([Bind("Name,Picture,Bio,CountryId,Id")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _artistService.Insert(artist);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_countryService.GetAll(), "Id", "Name", artist.CountryId);
            return View(artist);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid? id)
        {
            var artist = _artistService.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_countryService.GetAll(), "Id", "Name", artist.CountryId);
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid id, [Bind("Name,Picture,Bio,CountryId,Id")] Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _artistService.Update(artist);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_countryService.GetAll(), "Id", "Name", artist.CountryId);
            return View(artist);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(Guid? id)
        {
            var artist = _artistService.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _artistService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(Guid id)
        {
            return _artistService.GetById(id) != null;
        }
    }
}
