using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamDetailsAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new TeamCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TeamCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _teamService.CreateTeamAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null) return NotFound();

            return View(new TeamEditViewModel
            {
                Id = team.Id,
                Name = team.Name,
                HomeGround = team.HomeGround,
                Description = team.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, TeamEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _teamService.UpdateTeamAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _teamService.DeleteTeamAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
