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
        private readonly ITeamService _teamService;

        public PlayerController(IPlayerService playerService, ITeamService teamService)
        {
            _playerService = playerService;
            _teamService = teamService;
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
    }
}
