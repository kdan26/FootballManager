using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FootballManager.Controllers
{
    [Authorize]
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _matchService.GetAllMatchesAsync();
            return View(matches);
        }

        public async Task<IActionResult> Details(int id)
        {
            var match = await _matchService.GetMatchDetailsAsync(id);
            if (match == null) return NotFound();
            return View(match);
        }

        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create()
        {
            var teams = await _matchService.GetAllTeamsForSelectAsync();
            var model = new MatchCreateViewModel
            {
                Teams = teams.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(MatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teams = await _matchService.GetAllTeamsForSelectAsync();
                model.Teams = teams.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
                return View(model);
            }

            var (success, error) = await _matchService.CreateMatchAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                var teams = await _matchService.GetAllTeamsForSelectAsync();
                model.Teams = teams.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateResult(int id)
        {
            var match = await _matchService.GetMatchDetailsAsync(id);
            if (match == null) return NotFound();

            return View(new MatchResultViewModel
            {
                MatchId = match.Id,
                HomeTeamName = match.HomeTeamName,
                AwayTeamName = match.AwayTeamName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateResult(int id, MatchResultViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _matchService.UpdateResultAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var (success, error) = await _matchService.CancelMatchAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
