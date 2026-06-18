using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _db;

        public PlayerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<PlayerListItemViewModel>> GetPlayersByTeamAsync(int teamId)
        {
            return await _db.Players
                .Include(p => p.Team)
                .Where(p => p.TeamId == teamId)
                .OrderBy(p => p.JerseyNumber)
                .ThenBy(p => p.FullName)
                .Select(p => new PlayerListItemViewModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    JerseyNumber = p.JerseyNumber,
                    Position = p.Position,
                    IsActive = p.IsActive,
                    TeamId = p.TeamId,
                    TeamName = p.Team.Name
                })
                .ToListAsync();
        }

        public async Task<PlayerDetailsViewModel?> GetPlayerDetailsAsync(int id)
        {
            var p = await _db.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return null;

            return new PlayerDetailsViewModel
            {
                Id = p.Id,
                FullName = p.FullName,
                JerseyNumber = p.JerseyNumber,
                Position = p.Position,
                DateOfBirth = p.DateOfBirth,
                Nationality = p.Nationality,
                Notes = p.Notes,
                IsActive = p.IsActive,
                HealthStatus = p.HealthStatus,
                HealthNote = p.HealthNote,
                ExpectedReturnDate = p.ExpectedReturnDate,
                TeamId = p.TeamId,
                TeamName = p.Team.Name
            };
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _db.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreatePlayerAsync(PlayerCreateViewModel model)
        {
            // Kiểm tra số áo trùng trong cùng đội
            if (model.JerseyNumber.HasValue)
            {
                var exists = await _db.Players.AnyAsync(p =>
                    p.TeamId == model.TeamId && p.JerseyNumber == model.JerseyNumber);
                if (exists)
                    return (false, $"Số áo {model.JerseyNumber} đã được dùng trong đội này");
            }

            _db.Players.Add(new Player
            {
                FullName = model.FullName,
                JerseyNumber = model.JerseyNumber,
                Position = model.Position,
                DateOfBirth = model.DateOfBirth,
                Nationality = model.Nationality,
                Notes = model.Notes,
                IsActive = model.IsActive,
                TeamId = model.TeamId
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdatePlayerAsync(int id, PlayerEditViewModel model)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null)
                return (false, "Không tìm thấy cầu thủ");

            // Kiểm tra số áo trùng với cầu thủ khác trong cùng đội
            if (model.JerseyNumber.HasValue)
            {
                var exists = await _db.Players.AnyAsync(p =>
                    p.TeamId == model.TeamId && p.JerseyNumber == model.JerseyNumber && p.Id != id);
                if (exists)
                    return (false, $"Số áo {model.JerseyNumber} đã được dùng trong đội này");
            }

            player.FullName = model.FullName;
            player.JerseyNumber = model.JerseyNumber;
            player.Position = model.Position;
            player.DateOfBirth = model.DateOfBirth;
            player.Nationality = model.Nationality;
            player.Notes = model.Notes;
            player.IsActive = model.IsActive;
            player.HealthStatus = model.HealthStatus;
            player.HealthNote = model.HealthNote;
            player.ExpectedReturnDate = model.ExpectedReturnDate;

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletePlayerAsync(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null)
                return (false, "Không tìm thấy cầu thủ");

            // Xóa toàn bộ dữ liệu liên quan trước (các bảng Restrict FK)
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Điểm danh trận
                var playerAttendances = _db.PlayerAttendances.Where(a => a.PlayerId == id);
                _db.PlayerAttendances.RemoveRange(playerAttendances);

                // Chỉ số thi đấu
                var matchStats = _db.PlayerMatchStats.Where(s => s.PlayerId == id);
                _db.PlayerMatchStats.RemoveRange(matchStats);

                // Chỉ số tập luyện
                var trainingStats = _db.PlayerTrainingStats.Where(s => s.PlayerId == id);
                _db.PlayerTrainingStats.RemoveRange(trainingStats);

                // Điểm danh buổi tập
                var trainingAttendances = _db.TrainingAttendances.Where(a => a.PlayerId == id);
                _db.TrainingAttendances.RemoveRange(trainingAttendances);

                // Sự kiện trận đấu (ghi bàn, thẻ...) — xóa cả các event dùng player làm substitute
                var matchEvents = _db.MatchEvents
                    .Where(e => e.PlayerId == id || e.SubstitutePlayerId == id);
                _db.MatchEvents.RemoveRange(matchEvents);

                // Đánh giá phong độ
                var ratings = _db.PerformanceRatings.Where(r => r.PlayerId == id);
                _db.PerformanceRatings.RemoveRange(ratings);

                await _db.SaveChangesAsync();

                // Cuối cùng xóa cầu thủ
                _db.Players.Remove(player);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, $"Không thể xóa cầu thủ: {ex.Message}");
            }
        }

        public async Task UpdateHealthAsync(int playerId, PlayerHealthStatus status, string? note, DateTime? expectedReturn)
        {
            var player = await _db.Players.FindAsync(playerId);
            if (player == null) return;

            player.HealthStatus = status;
            player.HealthNote = note;
            player.ExpectedReturnDate = expectedReturn;
            await _db.SaveChangesAsync();
        }

        public async Task SaveTacticsAsync(int teamId, string formation, string? notes)
            => await SaveTacticsBoardAsync(teamId, formation, notes, null, null);

        public async Task SaveTacticsBoardAsync(int teamId, string formation,
            string? notes, string? positionsJson, string? arrowsJson)
        {
            var existing = await _db.TeamTactics.FirstOrDefaultAsync(t => t.TeamId == teamId);
            if (existing == null)
            {
                _db.TeamTactics.Add(new TeamTactics
                {
                    TeamId        = teamId,
                    Formation     = formation,
                    Notes         = notes,
                    PositionsJson = positionsJson,
                    ArrowsJson    = arrowsJson,
                    UpdatedAt     = DateTime.Now
                });
            }
            else
            {
                existing.Formation     = formation;
                existing.Notes         = notes;
                existing.UpdatedAt     = DateTime.Now;
                // Chỉ ghi đè JSON nếu có dữ liệu mới
                if (positionsJson != null) existing.PositionsJson = positionsJson;
                if (arrowsJson    != null) existing.ArrowsJson    = arrowsJson;
            }
            await _db.SaveChangesAsync();
        }

        public async Task<TeamTactics?> GetTacticsAsync(int teamId)
        {
            return await _db.TeamTactics.FirstOrDefaultAsync(t => t.TeamId == teamId);
        }
    }
}
