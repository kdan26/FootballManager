using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class PlayerStatsDetailService : IPlayerStatsDetailService
    {
        private readonly ApplicationDbContext _db;

        public PlayerStatsDetailService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ===================== MATCH STATS =====================

        public async Task<PlayerMatchStatsFormViewModel?> GetMatchStatsFormAsync(int playerId, int? matchId = null)
        {
            var player = await _db.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == playerId);
            if (player == null) return null;

            // Lấy các trận mà đội cầu thủ đã tham gia
            var matches = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamId == player.TeamId || m.AwayTeamId == player.TeamId)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            // Nếu đã có stats cho trận này thì load lên
            PlayerMatchStats? existing = null;
            if (matchId.HasValue)
            {
                existing = await _db.PlayerMatchStats
                    .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.MatchId == matchId.Value);
            }

            var vm = new PlayerMatchStatsFormViewModel
            {
                PlayerId = playerId,
                PlayerName = player.FullName,
                Position = player.Position,
                MatchId = matchId ?? 0,
                MatchLabel = matchId.HasValue
                    ? matches.Where(m => m.Id == matchId.Value)
                             .Select(m => $"{m.HomeTeam.Name} vs {m.AwayTeam.Name} ({m.MatchDate:dd/MM/yyyy})")
                             .FirstOrDefault() ?? ""
                    : "",
                Matches = matches.Select(m => new SelectListItem(
                    $"{m.HomeTeam.Name} vs {m.AwayTeam.Name} ({m.MatchDate:dd/MM/yyyy})",
                    m.Id.ToString()
                )).ToList()
            };

            if (existing != null)
            {
                vm.MinutesPlayed = existing.MinutesPlayed;
                vm.PassCompletionPct = existing.PassCompletionPct;
                vm.PsxGDiff = existing.PsxGDiff;
                vm.SavePct = existing.SavePct;
                vm.LaunchAccuracyPct = existing.LaunchAccuracyPct;
                vm.AccurateCrosses = existing.AccurateCrosses;
                vm.TacklesWonPct = existing.TacklesWonPct;
                vm.DistanceCovered = existing.DistanceCovered;
                vm.AerialDuelsPct = existing.AerialDuelsPct;
                vm.Clearances = existing.Clearances;
                vm.Interceptions = existing.Interceptions;
                vm.LongPassPct = existing.LongPassPct;
                vm.BallRecoveries = existing.BallRecoveries;
                vm.PressuredPassPct = existing.PressuredPassPct;
                vm.KeyPasses = existing.KeyPasses;
                vm.ShotCreatingActions = existing.ShotCreatingActions;
                vm.SuccessfulDribblesPct = existing.SuccessfulDribblesPct;
                vm.TouchesInPenaltyArea = existing.TouchesInPenaltyArea;
                vm.ShotsPer90 = existing.ShotsPer90;
                vm.ShotsOnTargetPct = existing.ShotsOnTargetPct;
                vm.ConversionRate = existing.ConversionRate;
                vm.Notes = existing.Notes;
            }

            return vm;
        }

        public async Task<List<PlayerMatchStatsListItem>> GetMatchStatsListAsync(int playerId)
        {
            return await _db.PlayerMatchStats
                .Include(s => s.Match).ThenInclude(m => m.HomeTeam)
                .Include(s => s.Match).ThenInclude(m => m.AwayTeam)
                .Where(s => s.PlayerId == playerId)
                .OrderByDescending(s => s.Match.MatchDate)
                .Select(s => new PlayerMatchStatsListItem
                {
                    Id = s.Id,
                    MatchId = s.MatchId,
                    MatchLabel = s.Match.HomeTeam.Name + " vs " + s.Match.AwayTeam.Name,
                    MatchDate = s.Match.MatchDate,
                    MinutesPlayed = s.MinutesPlayed,
                    PassCompletionPct = s.PassCompletionPct,
                    Summary = s.MinutesPlayed + " phút"
                })
                .ToListAsync();
        }

        public async Task SaveMatchStatsAsync(PlayerMatchStatsFormViewModel model)
        {
            var existing = await _db.PlayerMatchStats
                .FirstOrDefaultAsync(s => s.PlayerId == model.PlayerId && s.MatchId == model.MatchId);

            if (existing == null)
            {
                existing = new PlayerMatchStats { PlayerId = model.PlayerId, MatchId = model.MatchId };
                _db.PlayerMatchStats.Add(existing);
            }

            existing.MinutesPlayed = model.MinutesPlayed;
            existing.PassCompletionPct = model.PassCompletionPct;
            existing.PsxGDiff = model.PsxGDiff;
            existing.SavePct = model.SavePct;
            existing.LaunchAccuracyPct = model.LaunchAccuracyPct;
            existing.AccurateCrosses = model.AccurateCrosses;
            existing.TacklesWonPct = model.TacklesWonPct;
            existing.DistanceCovered = model.DistanceCovered;
            existing.AerialDuelsPct = model.AerialDuelsPct;
            existing.Clearances = model.Clearances;
            existing.Interceptions = model.Interceptions;
            existing.LongPassPct = model.LongPassPct;
            existing.BallRecoveries = model.BallRecoveries;
            existing.PressuredPassPct = model.PressuredPassPct;
            existing.KeyPasses = model.KeyPasses;
            existing.ShotCreatingActions = model.ShotCreatingActions;
            existing.SuccessfulDribblesPct = model.SuccessfulDribblesPct;
            existing.TouchesInPenaltyArea = model.TouchesInPenaltyArea;
            existing.ShotsPer90 = model.ShotsPer90;
            existing.ShotsOnTargetPct = model.ShotsOnTargetPct;
            existing.ConversionRate = model.ConversionRate;
            existing.Notes = model.Notes;
            existing.RecordedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        // ===================== TRAINING STATS =====================

        public async Task<List<PlayerTrainingStatsListItem>> GetTrainingStatsListAsync(int playerId)
        {
            return await _db.PlayerTrainingStats
                .Where(s => s.PlayerId == playerId)
                .OrderByDescending(s => s.TrainingDate)
                .Select(s => new PlayerTrainingStatsListItem
                {
                    Id = s.Id,
                    TrainingDate = s.TrainingDate,
                    DrillName = s.DrillName,
                    Category = s.Category,
                    CoachRating = s.CoachRating,
                    CoachNotes = s.CoachNotes
                })
                .ToListAsync();
        }

        public async Task<PlayerTrainingStatsFormViewModel> GetTrainingStatsFormAsync(int playerId)
        {
            var player = await _db.Players.FindAsync(playerId);
            return new PlayerTrainingStatsFormViewModel
            {
                PlayerId = playerId,
                PlayerName = player?.FullName ?? "",
                TrainingDate = DateTime.Today
            };
        }

        public async Task SaveTrainingStatsAsync(PlayerTrainingStatsFormViewModel model)
        {
            _db.PlayerTrainingStats.Add(new PlayerTrainingStats
            {
                PlayerId        = model.PlayerId,
                TrainingDate    = model.TrainingDate,
                Category        = model.Category,
                DrillName       = model.DrillName,
                DrillId         = model.DrillId,
                SprintSpeed     = model.SprintSpeed,
                EnduranceScore  = model.EnduranceScore,
                DistanceRun     = model.DistanceRun,
                TechniqueScore  = model.TechniqueScore,
                PassAccuracy    = model.PassAccuracy,
                ShotsOnTarget   = model.ShotsOnTarget,
                TacticsScore    = model.TacticsScore,
                CoachRating     = model.CoachRating,
                CoachNotes      = model.CoachNotes
            });
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTrainingStatsAsync(int id)
        {
            var s = await _db.PlayerTrainingStats.FindAsync(id);
            if (s != null)
            {
                _db.PlayerTrainingStats.Remove(s);
                await _db.SaveChangesAsync();
            }
        }

        // ===================== PERFORMANCE CHART =====================

        public async Task<PerformanceChartViewModel> GetPerformanceChartAsync(int playerId)
        {
            var player = await _db.Players.FindAsync(playerId);

            // Lấy training stats 30 buổi gần nhất, sắp xếp tăng dần (cũ → mới) cho chart
            var trainings = await _db.PlayerTrainingStats
                .Where(s => s.PlayerId == playerId)
                .OrderByDescending(s => s.TrainingDate)
                .Take(30)
                .ToListAsync();
            trainings.Reverse(); // cũ → mới cho chart

            // Lấy match stats 10 trận gần nhất
            var matchStats = await _db.PlayerMatchStats
                .Include(s => s.Match).ThenInclude(m => m.HomeTeam)
                .Include(s => s.Match).ThenInclude(m => m.AwayTeam)
                .Where(s => s.PlayerId == playerId)
                .OrderByDescending(s => s.Match.MatchDate)
                .Take(10)
                .ToListAsync();
            matchStats.Reverse(); // cũ → mới cho chart

            // Map training points
            var trainingPoints = trainings.Select(t => new TrainingChartPoint
            {
                Label = $"{t.TrainingDate:dd/MM} - {t.DrillName}",
                CoachRating = t.CoachRating,
                PhysicalScore = t.Category == TrainingCategory.Physical
                    ? t.EnduranceScore
                    : null,
                TechnicalScore = t.Category == TrainingCategory.Technical
                    ? t.TechniqueScore
                    : null,
                TacticalScore = t.Category == TrainingCategory.Tactical
                    ? t.TacticsScore
                    : null,
                Category = t.Category.ToString()
            }).ToList();

            // Map match points — chỉ số nổi bật tuỳ vị trí
            var pos = player?.Position ?? PlayerPosition.CentralMid;
            var matchPoints = matchStats.Select(s => {
                decimal? m1 = null, m2 = null;
                string l1 = "", l2 = "";

                switch (pos)
                {
                    case PlayerPosition.GoalKeeper:
                        m1 = s.SavePct; l1 = "% Cứu thua";
                        m2 = s.LaunchAccuracyPct; l2 = "% Chuyền dài";
                        break;
                    case PlayerPosition.CenterBack:
                        m1 = s.AerialDuelsPct; l1 = "% Không chiến";
                        m2 = s.TacklesWonPct; l2 = "% Tắc bóng";
                        break;
                    case PlayerPosition.FullBack:
                        m1 = s.TacklesWonPct; l1 = "% Tắc bóng";
                        m2 = s.DistanceCovered.HasValue ? (decimal?)s.DistanceCovered * 10 : null;
                        l2 = "Quãng đường (km×10)";
                        break;
                    case PlayerPosition.DefensiveMid:
                    case PlayerPosition.CentralMid:
                        m1 = s.PressuredPassPct; l1 = "% Chuyền khi pressing";
                        m2 = s.BallRecoveries.HasValue ? (decimal?)s.BallRecoveries : null;
                        l2 = "Thu hồi bóng";
                        break;
                    case PlayerPosition.AttackingMid:
                    case PlayerPosition.Winger:
                        m1 = s.SuccessfulDribblesPct; l1 = "% Đi bóng qua người";
                        m2 = s.KeyPasses.HasValue ? (decimal?)s.KeyPasses : null;
                        l2 = "Chuyền quyết định";
                        break;
                    case PlayerPosition.Striker:
                        m1 = s.ShotsOnTargetPct; l1 = "% Sút trúng đích";
                        m2 = s.ConversionRate; l2 = "Tỷ lệ ghi bàn";
                        break;
                }

                return new MatchChartPoint
                {
                    Label = $"{s.Match.HomeTeam.Name} vs {s.Match.AwayTeam.Name}\n{s.Match.MatchDate:dd/MM}",
                    PassCompletionPct = s.PassCompletionPct,
                    MinutesPlayed = s.MinutesPlayed,
                    KeyMetric1 = m1,
                    KeyMetric1Label = l1,
                    KeyMetric2 = m2,
                    KeyMetric2Label = l2
                };
            }).ToList();

            // Thống kê nhanh
            var avgCoach = trainings.Where(t => t.CoachRating.HasValue).ToList();
            var avgPass = matchStats.Where(m => m.PassCompletionPct.HasValue).ToList();

            decimal? ratingTrend = null;
            var rated = trainings.Where(t => t.CoachRating.HasValue).ToList();
            if (rated.Count >= 6)
            {
                var recent3Avg = rated.TakeLast(3).Average(t => (double)t.CoachRating!.Value);
                var prev3Avg   = rated.SkipLast(3).TakeLast(3).Average(t => (double)t.CoachRating!.Value);
                ratingTrend = (decimal)(recent3Avg - prev3Avg);
            }

            return new PerformanceChartViewModel
            {
                TrainingPoints = trainingPoints,
                MatchPoints = matchPoints,
                AvgCoachRating = avgCoach.Any()
                    ? Math.Round((decimal)avgCoach.Average(t => (double)t.CoachRating!.Value), 1)
                    : null,
                AvgPassCompletion = avgPass.Any()
                    ? Math.Round((decimal)avgPass.Average(m => (double)m.PassCompletionPct!.Value), 1)
                    : null,
                TotalTrainingSessions = trainings.Count,
                TotalMatchesPlayed = matchStats.Count,
                RatingTrend = ratingTrend.HasValue ? Math.Round(ratingTrend.Value, 1) : null
            };
        }
    }
}
