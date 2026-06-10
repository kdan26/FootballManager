using FootballManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public DashboardController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _statisticsService.GetDashboardDataAsync();
            return View(data);
        }
    }
}
