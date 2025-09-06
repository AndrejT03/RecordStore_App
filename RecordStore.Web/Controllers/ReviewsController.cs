using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.Identity;
using RecordStore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecordStore.Web.Controllers
{
    [Authorize] 
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RecordStoreApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<RecordStoreApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Rating, string? Comment, Guid RecordId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized(); 
                }

                var review = new Review
                {
                    Rating = Rating,
                    Comment = Comment,
                    RecordId = RecordId,
                    UserId = userId,
                    DateCreated = DateTime.Now
                };

                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Records", new { id = RecordId });
            }

            TempData["ErrorMessage"] = "There was an error submitting your review. Please try again.";
            return RedirectToAction("Details", "Records", new { id = RecordId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, int Rating, string? Comment, Guid RecordId)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("Details", "Records", new { id = RecordId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviewToUpdate = await _context.Reviews.FindAsync(id);

            if (reviewToUpdate == null)
            {
                return NotFound();
            }

            if (reviewToUpdate.UserId != userId)
            {
                return Forbid(); 
            }

            reviewToUpdate.Rating = Rating;
            reviewToUpdate.Comment = Comment;
            reviewToUpdate.DateModified = DateTime.Now;

            _context.Update(reviewToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Records", new { id = RecordId });
        }
    }
}