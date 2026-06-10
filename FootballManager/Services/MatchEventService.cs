using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class MatchEventService : IMatchEventService
    {
        private readonly ApplicationDbContext _db;

        public MatchEventService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<MatchEventsViewModel?> GetMatchEventsAsync(int matchId)
        {
            var match = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) return null;

            var events = await _db.MatchEvents
                .Include(e => e.Player).ThenInclude(p => p.Team)
                .Include(e => e.SubstitutePlayer)
                .Where(e => e.MatchId == matchId)
                .OrderBy(e => e.Minute)
                .ToListAsync();

            return new MatchEventsViewModel
            {
                MatchId = matchId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                MatchDate = match.MatchDate,
                Status = match.Status,
                Events = events.Select(e => new MatchEventListItem
                {
                    Id = e.Id,
                    Minute = e.Minute,
                    EventType = e.EventType,
                    PlayerName = e.Player.FullName,
                    SubstitutePlayerName = e.SubstitutePlayer?.FullName,
                    Description = e.Description,
                    TeamId = e.Player.TeamId
                }).ToList()
            };
        }

        public async Task<AddMatchEventViewModel?> GetAddEventFormAsync(int matchId)
        {
            var match = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) return null;

            var homePlayers = await _db.Players
                .Where(p => p.TeamId == match.HomeTeamId && p.IsActive)
                .OrderBy(p => p.JerseyNumber).ToListAsync();

            var awayPlayers = await _db.Players
                .Where(p => p.TeamId == match.AwayTeamId && p.IsActive)
                .OrderBy(p => p.JerseyNumber).ToListAsync();

            SelectListItem ToItem(Player p) =>
                new SelectListItem($"#{p.JerseyNumber} {p.FullName}", p.Id.ToString());

            var allPlayers = homePlayers.Concat(awayPlayers)
                .Select(ToItem).ToList();

            return new AddMatchEventViewModel
            {
                MatchId = matchId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomePlayers = homePlayers.Select(ToItem).ToList(),
                AwayPlayers = awayPlayers.Select(ToItem).ToList(),
                AllPlayers = allPlayers
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> AddEventAsync(AddMatchEventViewModel model)
        {
            var match = await _db.Matches.FindAsync(model.MatchId);
            if (match == null) return (false, "Không tìm thấy trận đấu");

            _db.MatchEvents.Add(new MatchEvent
            {
                MatchId = model.MatchId,
                PlayerId = model.PlayerId,
                SubstitutePlayerId = model.SubstitutePlayerId,
                EventType = model.EventType,
                Minute = model.Minute,
                Description = model.Description
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteEventAsync(int eventId)
        {
            var ev = await _db.MatchEvents.FindAsync(eventId);
            if (ev == null) return (false, "Không tìm thấy sự kiện");

            _db.MatchEvents.Remove(ev);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}
