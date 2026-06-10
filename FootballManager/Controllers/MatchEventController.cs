using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class MatchEventController : Controller
    {
        private readonly IMatchEventService _eventService;

        public MatchEventController(IMatchEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(int matchId)
        {
            var vm = await _eventService.GetMatchEventsAsync(matchId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Add(int matchId)
        {
            var vm = await _eventService.GetAddEventFormAsync(matchId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Add(AddMatchEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var form = await _eventService.GetAddEventFormAsync(model.MatchId);
                if (form != null)
                {
                    model.HomePlayers = form.HomePlayers;
                    model.AwayPlayers = form.AwayPlayers;
                    model.AllPlayers = form.AllPlayers;
                    model.HomeTeamName = form.HomeTeamName;
                    model.AwayTeamName = form.AwayTeamName;
                }
                return View(model);
            }

            var (success, error) = await _eventService.AddEventAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { matchId = model.MatchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Delete(int id, int matchId)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index), new { matchId });
        }
    }
}
