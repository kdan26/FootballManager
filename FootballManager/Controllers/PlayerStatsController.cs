using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Authorize]
    public class PlayerStatsController : Controller
    {
        private readonly IPlayerStatsDetailService _service;
        private readonly IPlayerService _playerService;

        public PlayerStatsController(IPlayerStatsDetailService service, IPlayerService playerService)
        {
            _service = service;
            _playerService = playerService;
        }

        // ===== TRANG CHỈ SỐ CẦU THỦ (click vào cầu thủ) =====
        public async Task<IActionResult> Index(int playerId)
        {
            var player = await _playerService.GetPlayerDetailsAsync(playerId);
            if (player == null) return NotFound();

            var matchStats = await _service.GetMatchStatsListAsync(playerId);
            var trainingStats = await _service.GetTrainingStatsListAsync(playerId);
            var chartData = await _service.GetPerformanceChartAsync(playerId);

            ViewBag.Player = player;
            ViewBag.MatchStats = matchStats;
            ViewBag.TrainingStats = trainingStats;
            ViewBag.ChartData = chartData;
            return View();
        }

        // ===== NHẬP CHỈ SỐ THI ĐẤU =====
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> MatchStats(int playerId, int? matchId)
        {
            var vm = await _service.GetMatchStatsFormAsync(playerId, matchId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> MatchStats(PlayerMatchStatsFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await _service.GetMatchStatsFormAsync(model.PlayerId, model.MatchId);
                if (refreshed != null) model.Matches = refreshed.Matches;
                return View(model);
            }

            await _service.SaveMatchStatsAsync(model);
            TempData["Success"] = "Đã lưu chỉ số thi đấu";
            return RedirectToAction(nameof(Index), new { playerId = model.PlayerId });
        }

        // ===== NHẬP CHỈ SỐ BÀI TẬP =====
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Training(int playerId)
        {
            var vm = await _service.GetTrainingStatsFormAsync(playerId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Training(PlayerTrainingStatsFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _service.SaveTrainingStatsAsync(model);
            TempData["Success"] = "Đã lưu chỉ số bài tập";
            return RedirectToAction(nameof(Index), new { playerId = model.PlayerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> DeleteTraining(int id, int playerId)
        {
            await _service.DeleteTrainingStatsAsync(id);
            return RedirectToAction(nameof(Index), new { playerId });
        }

        // ===== CẬP NHẬT TRẠNG THÁI SỨC KHỎE =====
        // Redirect về Index thay vì render view riêng không tồn tại
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateHealth(int playerId)
        {
            var player = await _playerService.GetPlayerDetailsAsync(playerId);
            if (player == null) return NotFound();
            // Render thẳng form inline trong Index — redirect về Index với fragment
            TempData["ShowHealthForm"] = true;
            return RedirectToAction(nameof(Index), new { playerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> UpdateHealth(int playerId,
            FootballManager.Models.PlayerHealthStatus healthStatus,
            string? healthNote,
            DateTime? expectedReturnDate)
        {
            await _playerService.UpdateHealthAsync(playerId, healthStatus, healthNote, expectedReturnDate);
            TempData["Success"] = "Đã cập nhật trạng thái sức khỏe";
            return RedirectToAction(nameof(Index), new { playerId });
        }

        // ===== SƠ ĐỒ CHIẾN THUẬT (TEXT) — redirect sang TacticalBoard =====
        [Authorize(Roles = "Admin,Coach")]
        public IActionResult Tactics(int teamId)
        {
            return RedirectToAction(nameof(TacticalBoard), new { teamId });
        }

        // ===== TACTICAL BOARD (CANVAS) =====
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> TacticalBoard(int teamId)
        {
            var players  = await _playerService.GetPlayersByTeamAsync(teamId);
            var tactics  = await _playerService.GetTacticsAsync(teamId);
            ViewBag.TeamId       = teamId;
            ViewBag.Formation    = tactics?.Formation    ?? "4-3-3";
            ViewBag.Notes        = tactics?.Notes        ?? "";
            ViewBag.PositionsJson = tactics?.PositionsJson ?? "[]";
            ViewBag.ArrowsJson   = tactics?.ArrowsJson   ?? "[]";
            return View(players);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> SaveTacticalBoard(int teamId, string formation,
            string? notes, string? positionsJson, string? arrowsJson)
        {
            await _playerService.SaveTacticsBoardAsync(teamId, formation, notes, positionsJson, arrowsJson);
            return Ok(new { success = true });
        }
    }
}
