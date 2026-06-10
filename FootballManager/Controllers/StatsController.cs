using FootballManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private readonly IPlayerStatsService _statsService;

        public StatsController(IPlayerStatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task<IActionResult> Index(string? team = null)
        {
            var vm = await _statsService.GetStatsAsync(team);
            return View(vm);
        }

        public async Task<IActionResult> Player(int id)
        {
            var stats = await _statsService.GetPlayerStatsAsync(id);
            if (stats == null) return NotFound();
            return View(stats);
        }
    }
}
