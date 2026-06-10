using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class PerformanceRatingService : IPerformanceRatingService
    {
        private readonly ApplicationDbContext _db;

        public PerformanceRatingService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // INDEX: danh sách + thống kê + chart
        // ============================================================
        public async Task<PerformanceRatingIndexViewModel?> GetIndexAsync(int playerId)
        {
            var player = await _db.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null) return null;

            var ratings = await _db.PerformanceRatings
                .Include(r => r.RatedByUser)
                .Include(r => r.Match).ThenInclude(m => m!.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m!.AwayTeam)
                .Where(r => r.PlayerId == playerId)
                .OrderByDescending(r => r.RatingDate)
                .ToListAsync();

            var listItems = ratings.Select(r => new PerformanceRatingListItem
            {
                Id                  = r.Id,
                RatingType          = r.RatingType,
                RatingDate          = r.RatingDate,
                OverallRating       = r.OverallRating,
                AttitudeRating      = r.AttitudeRating,
                FitnessRating       = r.FitnessRating,
                TechnicalRating     = r.TechnicalRating,
                TacticalRating      = r.TacticalRating,
                Notes               = r.Notes,
                RatedByName         = r.RatedByUser.FullName,
                IsPublishedToPlayer = r.IsPublishedToPlayer,
                MatchLabel          = r.Match != null
                    ? $"{r.Match.HomeTeam.Name} vs {r.Match.AwayTeam.Name} ({r.Match.MatchDate:dd/MM/yyyy})"
                    : null
            }).ToList();

            // Chart: lấy 20 đánh giá gần nhất, sắp tăng dần (cũ → mới)
            var chartData = ratings
                .OrderBy(r => r.RatingDate)
                .TakeLast(20)
                .Select(r => new RatingChartPoint
                {
                    Label = r.RatingDate.ToString("dd/MM") + " - " + r.RatingType switch
                    {
                        RatingType.AfterMatch    => "Sau trận",
                        RatingType.AfterTraining => "Sau tập",
                        RatingType.Weekly        => "Tuần",
                        RatingType.Monthly       => "Tháng",
                        _                        => ""
                    },
                    OverallRating       = r.OverallRating,
                    AttitudeRating      = r.AttitudeRating,
                    FitnessRating       = r.FitnessRating,
                    TechnicalRating     = r.TechnicalRating,
                    TacticalRating      = r.TacticalRating,
                    RatingTypeLabel     = r.RatingType.ToString()
                }).ToList();

            // Thống kê trung bình
            decimal? Avg(Func<PerformanceRating, decimal?> selector)
            {
                var vals = ratings.Select(selector).Where(v => v.HasValue).Select(v => v!.Value).ToList();
                return vals.Any() ? Math.Round(vals.Average(), 1) : null;
            }

            var avgOverall = ratings.Any()
                ? Math.Round(ratings.Average(r => (double)r.OverallRating), 1)
                : (double?)null;

            // Xu hướng: 3 gần nhất so với 3 trước đó
            decimal? trend = null;
            var sorted = ratings.OrderBy(r => r.RatingDate).ToList();
            if (sorted.Count >= 6)
            {
                var recent3 = sorted.TakeLast(3).Average(r => (double)r.OverallRating);
                var prev3   = sorted.SkipLast(3).TakeLast(3).Average(r => (double)r.OverallRating);
                trend = Math.Round((decimal)(recent3 - prev3), 1);
            }

            string posLabel = player.Position switch
            {
                PlayerPosition.GoalKeeper   => "Thủ môn",
                PlayerPosition.CenterBack   => "Trung vệ",
                PlayerPosition.FullBack     => "Hậu vệ biên",
                PlayerPosition.DefensiveMid => "Tiền vệ phòng ngự",
                PlayerPosition.CentralMid   => "Tiền vệ trung tâm",
                PlayerPosition.AttackingMid => "Tiền vệ công",
                PlayerPosition.Winger       => "Chạy cánh",
                PlayerPosition.Striker      => "Tiền đạo",
                _                           => ""
            };

            return new PerformanceRatingIndexViewModel
            {
                PlayerId      = playerId,
                PlayerName    = player.FullName,
                PositionLabel = posLabel,
                TeamName      = player.Team.Name,
                JerseyNumber  = player.JerseyNumber,
                Ratings       = listItems,
                ChartPoints   = chartData,
                AvgOverall    = avgOverall.HasValue ? (decimal)avgOverall.Value : null,
                AvgAttitude   = Avg(r => r.AttitudeRating),
                AvgFitness    = Avg(r => r.FitnessRating),
                AvgTechnical  = Avg(r => r.TechnicalRating),
                AvgTactical   = Avg(r => r.TacticalRating),
                Trend         = trend,
                TotalRatings  = ratings.Count
            };
        }

        // ============================================================
        // FORM: thêm mới hoặc load sửa
        // ============================================================
        public async Task<PerformanceRatingFormViewModel?> GetFormAsync(int playerId, int? ratingId = null)
        {
            var player = await _db.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);
            if (player == null) return null;

            // Dropdown các trận của đội
            var matches = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamId == player.TeamId || m.AwayTeamId == player.TeamId)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            var matchItems = matches.Select(m => new SelectListItem(
                $"{m.HomeTeam.Name} vs {m.AwayTeam.Name} ({m.MatchDate:dd/MM/yyyy})",
                m.Id.ToString()
            )).ToList();

            // Load bản ghi nếu đang sửa
            PerformanceRating? existing = null;
            if (ratingId.HasValue)
            {
                existing = await _db.PerformanceRatings.FindAsync(ratingId.Value);
                if (existing == null || existing.PlayerId != playerId) return null;
            }

            return new PerformanceRatingFormViewModel
            {
                Id                  = existing?.Id ?? 0,
                PlayerId            = playerId,
                PlayerName          = player.FullName,
                RatingType          = existing?.RatingType ?? RatingType.AfterMatch,
                MatchId             = existing?.MatchId,
                RatingDate          = existing?.RatingDate ?? DateTime.Today,
                OverallRating       = existing?.OverallRating ?? 7,
                AttitudeRating      = existing?.AttitudeRating,
                FitnessRating       = existing?.FitnessRating,
                TechnicalRating     = existing?.TechnicalRating,
                TacticalRating      = existing?.TacticalRating,
                Notes               = existing?.Notes,
                IsPublishedToPlayer = existing?.IsPublishedToPlayer ?? false,
                Matches             = matchItems
            };
        }

        // ============================================================
        // SAVE: thêm mới hoặc cập nhật
        // ============================================================
        public async Task<int> SaveAsync(PerformanceRatingFormViewModel model, int ratedByUserId)
        {
            PerformanceRating entity;

            if (model.Id > 0)
            {
                entity = await _db.PerformanceRatings.FindAsync(model.Id)
                    ?? throw new InvalidOperationException("Không tìm thấy đánh giá");
            }
            else
            {
                entity = new PerformanceRating { PlayerId = model.PlayerId };
                _db.PerformanceRatings.Add(entity);
            }

            entity.RatedByUserId        = ratedByUserId;
            entity.RatingType           = model.RatingType;
            entity.MatchId              = model.RatingType == RatingType.AfterMatch ? model.MatchId : null;
            entity.RatingDate           = model.RatingDate;
            entity.OverallRating        = model.OverallRating;
            entity.AttitudeRating       = model.AttitudeRating;
            entity.FitnessRating        = model.FitnessRating;
            entity.TechnicalRating      = model.TechnicalRating;
            entity.TacticalRating       = model.TacticalRating;
            entity.Notes                = model.Notes;
            entity.IsPublishedToPlayer  = model.IsPublishedToPlayer;

            await _db.SaveChangesAsync();
            return entity.Id;
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task DeleteAsync(int ratingId)
        {
            var entity = await _db.PerformanceRatings.FindAsync(ratingId);
            if (entity != null)
            {
                _db.PerformanceRatings.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        // ============================================================
        // TOGGLE PUBLISH
        // ============================================================
        public async Task TogglePublishAsync(int ratingId)
        {
            var entity = await _db.PerformanceRatings.FindAsync(ratingId);
            if (entity != null)
            {
                entity.IsPublishedToPlayer = !entity.IsPublishedToPlayer;
                await _db.SaveChangesAsync();
            }
        }
    }
}
