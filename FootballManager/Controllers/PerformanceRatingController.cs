using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballManager.Controllers
{
    [Authorize]
    public class PerformanceRatingController : Controller
    {
        private readonly IPerformanceRatingService _service;

        public PerformanceRatingController(IPerformanceRatingService service)
        {
            _service = service;
        }

        // GET: /PerformanceRating?playerId=5
        public async Task<IActionResult> Index(int playerId)
        {
            var vm = await _service.GetIndexAsync(playerId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET: /PerformanceRating/Create?playerId=5
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(int playerId)
        {
            var vm = await _service.GetFormAsync(playerId);
            if (vm == null) return NotFound();
            return View("Form", vm);
        }

        // GET: /PerformanceRating/Edit/10?playerId=5
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id, int playerId)
        {
            var vm = await _service.GetFormAsync(playerId, id);
            if (vm == null) return NotFound();
            return View("Form", vm);
        }

        // POST: /PerformanceRating/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Save(PerformanceRatingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown
                var refreshed = await _service.GetFormAsync(model.PlayerId, model.Id > 0 ? model.Id : null);
                if (refreshed != null) model.Matches = refreshed.Matches;
                return View("Form", model);
            }

            var userId = GetCurrentUserId();
            if (userId == null) return Forbid();

            await _service.SaveAsync(model, userId.Value);
            TempData["Success"] = model.Id > 0
                ? "Đã cập nhật đánh giá phong độ"
                : "Đã thêm đánh giá phong độ";

            return RedirectToAction(nameof(Index), new { playerId = model.PlayerId });
        }

        // POST: /PerformanceRating/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Delete(int id, int playerId)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Đã xóa đánh giá";
            return RedirectToAction(nameof(Index), new { playerId });
        }

        // POST: /PerformanceRating/TogglePublish
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> TogglePublish(int id, int playerId)
        {
            await _service.TogglePublishAsync(id);
            return RedirectToAction(nameof(Index), new { playerId });
        }

        // ── Helper ──────────────────────────────────────────────────
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
