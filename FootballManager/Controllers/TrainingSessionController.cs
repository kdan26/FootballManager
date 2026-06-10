using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballManager.Controllers
{
    [Authorize]
    public class TrainingSessionController : Controller
    {
        private readonly ITrainingSessionService _service;

        public TrainingSessionController(ITrainingSessionService service)
        {
            _service = service;
        }

        // GET /TrainingSession
        public async Task<IActionResult> Index(int? teamId)
        {
            var vm = await _service.GetSessionIndexAsync(teamId);
            return View(vm);
        }

        // GET /TrainingSession/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.GetSessionDetailsAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET /TrainingSession/Create?teamId=1
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(int? teamId)
        {
            var vm = await _service.GetSessionFormAsync(null, teamId);
            return View("Form", vm);
        }

        // GET /TrainingSession/Edit/5
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetSessionFormAsync(id, null);
            return View("Form", vm);
        }

        // POST /TrainingSession/Save
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Save(TrainingSessionFormViewModel model)
        {
            // SelectedDrillIds không bắt buộc
            ModelState.Remove("SelectedDrillIds");

            if (!ModelState.IsValid)
            {
                var refreshed = await _service.GetSessionFormAsync(
                    model.Id > 0 ? model.Id : null, model.TeamId);
                model.Teams           = refreshed.Teams;
                model.AvailableDrills = refreshed.AvailableDrills;
                return View("Form", model);
            }

            var userId = GetUserId();
            if (userId == null) return Forbid();

            var id = await _service.SaveSessionAsync(model, userId.Value);
            TempData["Success"] = model.Id > 0 ? "Đã cập nhật buổi tập" : "Đã tạo buổi tập mới";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /TrainingSession/Delete
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSessionAsync(id);
            TempData["Success"] = "Đã xóa buổi tập";
            return RedirectToAction(nameof(Index));
        }

        // POST /TrainingSession/Complete
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Complete(int id)
        {
            await _service.CompleteSessionAsync(id);
            TempData["Success"] = "Đã đánh dấu hoàn thành";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /TrainingSession/Cancel
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelSessionAsync(id);
            TempData["Success"] = "Đã hủy buổi tập";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /TrainingSession/RemoveDrill
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> RemoveDrill(int sessionDrillId, int sessionId)
        {
            await _service.RemoveDrillFromSessionAsync(sessionDrillId);
            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        private int? GetUserId()
        {
            var v = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(v, out var id) ? id : null;
        }
    }
}
