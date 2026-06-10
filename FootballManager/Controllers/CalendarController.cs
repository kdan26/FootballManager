using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ICalendarService _calendarService;
        private readonly ITeamService     _teamService;

        public CalendarController(ICalendarService calendarService, ITeamService teamService)
        {
            _calendarService = calendarService;
            _teamService     = teamService;
        }

        // GET /Calendar
        public async Task<IActionResult> Index(int? teamId)
        {
            var teams = await _teamService.GetAllTeamsAsync();
            ViewBag.Teams  = teams;
            ViewBag.TeamId = teamId;
            return View();
        }

        // GET /Calendar/Events?teamId=1&start=2026-06-01&end=2026-06-30
        // — AJAX endpoint cho FullCalendar
        public async Task<IActionResult> Events(int? teamId, DateTime start, DateTime end)
        {
            var events = await _calendarService.GetEventsAsync(teamId, start, end);
            return Json(events);
        }

        // GET /Calendar/Attendance/5
        public async Task<IActionResult> Attendance(int id)
        {
            var vm = await _calendarService.GetAttendanceAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // POST /Calendar/SaveAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> SaveAttendance(SaveTrainingAttendanceViewModel model)
        {
            await _calendarService.SaveAttendanceAsync(model);
            TempData["Success"] = "Đã lưu điểm danh buổi tập";
            return RedirectToAction(nameof(Attendance), new { id = model.SessionId });
        }
    }
}
