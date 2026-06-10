using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class PlayerStatsService : IPlayerStatsService
    {
        private readonly ApplicationDbContext _db;

        public PlayerStatsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PlayerStatsViewModel> GetStatsAsync(string? teamName = null)
        {
            // Lấy tất cả events từ các trận đã hoàn thành
            var eventsQuery = _db.MatchEvents
                .Include(e => e.Player).ThenInclude(p => p.Team)
                .Include(e => e.Match)
                .Where(e => e.Match.Status == MatchStatus.Completed);

            var events = await eventsQuery.ToListAsync();

            // Lấy danh sách teams để filter
            var teams = await _db.Teams.Select(t => t.Name).OrderBy(n => n).ToListAsync();

            // Filter theo team nếu có
            if (!string.IsNullOrEmpty(teamName))
                events = events.Where(e => e.Player.Team.Name == teamName).ToList();

            // Group theo cầu thủ
            var grouped = events
                .GroupBy(e => e.Player)
                .Select(g => new PlayerStatsItem
                {
                    PlayerId = g.Key.Id,
                    FullName = g.Key.FullName,
                    JerseyNumber = g.Key.JerseyNumber,
                    PositionLabel = g.Key.Position switch
                    {
                        PlayerPosition.GoalKeeper   => "Thủ môn",
                        PlayerPosition.CenterBack   => "Trung vệ",
                        PlayerPosition.FullBack     => "Hậu vệ biên",
                        PlayerPosition.DefensiveMid => "Tiền vệ phòng ngự",
                        PlayerPosition.CentralMid   => "Tiền vệ trung tâm",
                        PlayerPosition.AttackingMid => "Tiền vệ công",
                        PlayerPosition.Winger       => "Chạy cánh",
                        PlayerPosition.Striker      => "Tiền đạo",
                        _ => ""
                    },
                    TeamName = g.Key.Team.Name,
                    Goals = g.Count(e => e.EventType == MatchEventType.Goal),
                    OwnGoals = g.Count(e => e.EventType == MatchEventType.OwnGoal),
                    Penalties = g.Count(e => e.EventType == MatchEventType.Penalty),
                    YellowCards = g.Count(e => e.EventType == MatchEventType.YellowCard),
                    RedCards = g.Count(e => e.EventType == MatchEventType.RedCard),
                    MatchesPlayed = g.Select(e => e.MatchId).Distinct().Count()
                })
                .ToList();

            return new PlayerStatsViewModel
            {
                AllStats    = grouped.OrderByDescending(p => p.TotalGoals).ToList(),
                TopScorers  = grouped.Where(p => p.TotalGoals > 0)
                    .OrderByDescending(p => p.TotalGoals).Take(10).ToList(),
                MostYellowCards = grouped.Where(p => p.YellowCards > 0)
                    .OrderByDescending(p => p.YellowCards).Take(10).ToList(),
                MostRedCards = grouped.Where(p => p.RedCards > 0)
                    .OrderByDescending(p => p.RedCards).Take(10).ToList(),
                TeamFilter = teamName,
                Teams = teams
            };
        }

        public async Task<PlayerStatsItem?> GetPlayerStatsAsync(int playerId)
        {
            var player = await _db.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null) return null;

            var events = await _db.MatchEvents
                .Include(e => e.Match)
                .Where(e => e.PlayerId == playerId && e.Match.Status == MatchStatus.Completed)
                .ToListAsync();

            return new PlayerStatsItem
            {
                PlayerId = player.Id,
                FullName = player.FullName,
                JerseyNumber = player.JerseyNumber,
                PositionLabel = player.Position switch
                {
                    PlayerPosition.GoalKeeper   => "Thủ môn",
                    PlayerPosition.CenterBack   => "Trung vệ",
                    PlayerPosition.FullBack     => "Hậu vệ biên",
                    PlayerPosition.DefensiveMid => "Tiền vệ phòng ngự",
                    PlayerPosition.CentralMid   => "Tiền vệ trung tâm",
                    PlayerPosition.AttackingMid => "Tiền vệ công",
                    PlayerPosition.Winger       => "Chạy cánh",
                    PlayerPosition.Striker      => "Tiền đạo",
                    _ => ""
                },
                TeamName = player.Team.Name,
                Goals = events.Count(e => e.EventType == MatchEventType.Goal),
                OwnGoals = events.Count(e => e.EventType == MatchEventType.OwnGoal),
                Penalties = events.Count(e => e.EventType == MatchEventType.Penalty),
                YellowCards = events.Count(e => e.EventType == MatchEventType.YellowCard),
                RedCards = events.Count(e => e.EventType == MatchEventType.RedCard),
                MatchesPlayed = events.Select(e => e.MatchId).Distinct().Count()
            };
        }
    }
}
