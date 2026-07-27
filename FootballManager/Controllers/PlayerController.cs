using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly ITeamService   _teamService;
        private readonly IWebHostEnvironment _env;

        public PlayerController(IPlayerService playerService, ITeamService teamService,
            IWebHostEnvironment env)
        {
            _playerService = playerService;
            _teamService   = teamService;
            _env           = env;
        }

        // GET /Player?teamId=1
        public async Task<IActionResult> Index(int teamId)
        {
            var team = await _teamService.GetTeamByIdAsync(teamId);
            if (team == null) return NotFound();

            var players = await _playerService.GetPlayersByTeamAsync(teamId);
            ViewBag.TeamId = teamId;
            ViewBag.TeamName = team.Name;
            return View(players);
        }

        public async Task<IActionResult> Details(int id)
        {
            var player = await _playerService.GetPlayerDetailsAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(int teamId)
        {
            var team = await _teamService.GetTeamByIdAsync(teamId);
            if (team == null) return NotFound();

            return View(new PlayerCreateViewModel
            {
                TeamId = teamId,
                TeamName = team.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(PlayerCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _playerService.CreatePlayerAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { teamId = model.TeamId });
        }

        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null) return NotFound();

            return View(new PlayerEditViewModel
            {
                Id = player.Id,
                FullName = player.FullName,
                JerseyNumber = player.JerseyNumber,
                Position = player.Position,
                DateOfBirth = player.DateOfBirth,
                Nationality = player.Nationality,
                Notes = player.Notes,
                IsActive = player.IsActive,
                HealthStatus = player.HealthStatus,
                HealthNote = player.HealthNote,
                ExpectedReturnDate = player.ExpectedReturnDate,
                TeamId = player.TeamId,
                TeamName = player.Team?.Name ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id, PlayerEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _playerService.UpdatePlayerAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { teamId = model.TeamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Delete(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null) return NotFound();
            int teamId = player.TeamId;

            var (success, error) = await _playerService.DeletePlayerAsync(id);
            if (!success)
                TempData["Error"] = error;

            return RedirectToAction(nameof(Index), new { teamId });
        }

        // POST /Player/UploadPhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file ảnh";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Giới hạn 2MB
            if (photo.Length > 2 * 1024 * 1024)
            {
                TempData["Error"] = "Ảnh không được vượt quá 2MB";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                var url = await _playerService.SavePlayerPhotoAsync(id, photo, _env.WebRootPath);
                if (url == null)
                {
                    TempData["Error"] = "Định dạng không hợp lệ. Chỉ chấp nhận JPG, PNG, WEBP";
                }
                else
                {
                    TempData["Success"] = "Đã cập nhật ảnh đại diện";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi upload: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
