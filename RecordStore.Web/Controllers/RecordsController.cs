using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using RecordStore.Repository;
using RecordStore.Repository.Interface;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecordStore.Web.Controllers
{
    public class RecordsController : Controller
    {
        private readonly IRecordService _recordService;
        private readonly IArtistService _artistService;
        private readonly IRepository<RecordLabel> _recordLabelRepository;

        public RecordsController(IRecordService recordService, IArtistService artistService, IRepository<RecordLabel> recordLabelRepository)
        {
            _recordService = recordService;
            _artistService = artistService;
            _recordLabelRepository = recordLabelRepository;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, Genre? filterGenre, RecordFormat? filterFormat, bool? filterIsReissue, int? page)
        {
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchString = searchString;
            ViewBag.filterGenre = filterGenre;
            ViewBag.filterFormat = filterFormat;
            ViewBag.filterIsReissue = filterIsReissue;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var records = await _recordService.GetAllPagedAndSortedAsync(
                sortOrder,
                searchString,
                filterGenre,
                filterFormat,
                filterIsReissue,
                pageNumber,
                pageSize
            );

            return View(records);
        }

        public IActionResult Details(Guid? id)
        {
            var record = _recordService.GetById(id.Value);
            if (record == null)
            {
                return NotFound();
            }
            if (record.Reviews != null && record.Reviews.Any())
            {
                ViewData["AverageRating"] = record.Reviews.Average(r => r.Rating).ToString("0.0");
            }
            else
            {
                ViewData["AverageRating"] = "N/A";
            }
            return View(record);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_artistService.GetAll(), "Id", "Name");
            ViewData["RecordLabelId"] = new SelectList(_recordLabelRepository.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([Bind("Title,ArtistId,ReleaseYear,Format,Genre,RecordLabelId,IsReissue,CoverURL,Price,StockQuantity")] Record record)
        {
            if (ModelState.IsValid)
            {
                _recordService.Insert(record);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_artistService.GetAll(), "Id", "Name", record.ArtistId);
            ViewData["RecordLabelId"] = new SelectList(_recordLabelRepository.GetAll(), "Id", "Name", record.RecordLabelId);
            return View(record);
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid? id)
        {
            var record = _recordService.GetById(id.Value);
            if (record == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_artistService.GetAll(), "Id", "Name", record.ArtistId);
            ViewData["RecordLabelId"] = new SelectList(_recordLabelRepository.GetAll(), "Id", "Name", record.RecordLabelId);
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid id, [Bind("Id,Title,ArtistId,ReleaseYear,Format,Genre,RecordLabelId,IsReissue,CoverURL,Price,StockQuantity")] Record record)
        {
            if (id != record.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recordService.Update(record);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_recordService.GetById(record.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistId"] = new SelectList(_artistService.GetAll(), "Id", "Name", record.ArtistId);
            ViewData["RecordLabelId"] = new SelectList(_recordLabelRepository.GetAll(), "Id", "Name", record.RecordLabelId);
            return View(record);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(Guid? id)
        {
            var record = _recordService.GetById(id.Value);
            if (record == null)
            {
                return NotFound();
            }
            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _recordService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult AddRecordToCart(Guid id)
        {
            var addToCartDto = _recordService.GetSelectedShoppingCartProduct(id);
            if (addToCartDto == null)
            {
                return NotFound();
            }
            return View(addToCartDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRecordToCart([Bind("RecordId,Quantity")] AddToCartDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                _recordService.AddProductToSoppingCart(model.RecordId, Guid.Parse(userId), model.Quantity);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while adding the item to the cart. {ex.Message} Please try again.";
                return RedirectToAction("Details", new { id = model.RecordId });
            }

            return RedirectToAction("Index", "ShoppingCarts");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult AddTracklist(AddTracklistDTO dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _recordService.AddTracksToRecord(dto);
                    TempData["SuccessMessage"] = "Tracklist added successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
                return RedirectToAction("Details", new { id = dto.RecordId });
            }
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["ErrorMessage"] = "Validation failed: " + string.Join("; ", errorMessages);
                return RedirectToAction("Details", new { id = dto.RecordId });
            }
            return RedirectToAction("Details", new { id = dto.RecordId });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTrack(Guid trackId, Guid recordId)
        {
            try
            {
                _recordService.DeleteTrack(trackId);
                TempData["SuccessMessage"] = "Track deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting track: {ex.Message}";
            }
            return RedirectToAction("Details", new { id = recordId });
        }
    }
}
