using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;

        public AttendanceController(
            IAttendanceService attendanceService,
            IMatchService matchService,
            ITeamService teamService)
        {
            _attendanceService = attendanceService;
            _matchService = matchService;
            _teamService = teamService;
        }

        // GET /Attendance/Match/5?teamId=1
        public async Task<IActionResult> Match(int id, int teamId)
        {
            var vm = await _attendanceService.GetMatchAttendanceAsync(id, teamId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // POST /Attendance/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Save(SaveAttendanceViewModel model)
        {
            await _attendanceService.SaveAttendanceAsync(model);
            TempData["Success"] = "Đã lưu điểm danh";
            return RedirectToAction(nameof(Match), new { id = model.MatchId, teamId = model.TeamId });
        }
    }
}
