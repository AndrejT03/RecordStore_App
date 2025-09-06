using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;

namespace RecordStore.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RecordLabelsController : Controller
    {
        private readonly IRecordLabelService _recordLabelService;
        private readonly ICountryService _countryService;

        public RecordLabelsController(IRecordLabelService recordLabelService, ICountryService countryService)
        {
            _recordLabelService = recordLabelService;
            _countryService = countryService;
        }

        public IActionResult Index()
        {
            return View(_recordLabelService.GetAll());
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();
            var recordLabel = _recordLabelService.GetDetails(id);
            if (recordLabel == null) return NotFound();
            return View(recordLabel);
        }

        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_countryService.GetAll().OrderBy(c => c.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,CountryId,City")] RecordLabel recordLabel)
        {
            if (ModelState.IsValid)
            {
                recordLabel.Id = Guid.NewGuid();
                _recordLabelService.Create(recordLabel);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_countryService.GetAll().OrderBy(c => c.Name), "Id", "Name", recordLabel.CountryId);
            return View(recordLabel);
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var recordLabel = _recordLabelService.GetDetails(id);
            if (recordLabel == null) return NotFound();
            ViewData["CountryId"] = new SelectList(_countryService.GetAll().OrderBy(c => c.Name), "Id", "Name", recordLabel.CountryId);
            return View(recordLabel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Name,CountryId,City")] RecordLabel recordLabel)
        {
            if (id != recordLabel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _recordLabelService.Update(recordLabel);
                }
                catch
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_countryService.GetAll().OrderBy(c => c.Name), "Id", "Name", recordLabel.CountryId);
            return View(recordLabel);
        }

        public IActionResult Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var recordLabel = _recordLabelService.GetDetails(id);
            if (recordLabel == null) return NotFound();
            return View(recordLabel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _recordLabelService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}