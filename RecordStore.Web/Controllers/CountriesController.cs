using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository;
using RecordStore.Service.Implementation;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RecordStore.Web.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ICountryService _service;

        public CountriesController(ICountryService countriesService)
        {
            _service = countriesService;
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

            var countries = await _service.GetAllCountriesPagedAndSortedAsync(sortOrder, searchString, pageNumber, pageSize);

            return View(countries);
        }

        public IActionResult Details(Guid? id)
        {
            var country = _service.GetById(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Code1,Code2,Capital,Region,Flag")] Country country)
        {
            if (ModelState.IsValid)
            {
                _service.Insert(country);
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        public IActionResult Edit(Guid? id)
        {
            var country = _service.GetById(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id, Name,Code1,Code2,Capital,Region,Flag")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }
            _service.Update(country);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(Guid? id)
        {
            var country = _service.GetById(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _service.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
