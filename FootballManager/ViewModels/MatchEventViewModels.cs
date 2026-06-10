using FootballManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class MatchEventListItem
    {
        public int Id { get; set; }
        public int? Minute { get; set; }
        public MatchEventType EventType { get; set; }
        public string EventLabel => EventType switch
        {
            MatchEventType.Goal          => "⚽ Bàn thắng",
            MatchEventType.OwnGoal       => "🥅 Phản lưới",
            MatchEventType.YellowCard    => "🟨 Thẻ vàng",
            MatchEventType.RedCard       => "🟥 Thẻ đỏ",
            MatchEventType.Substitution  => "🔄 Thay người",
            MatchEventType.Penalty       => "⚽ Phạt đền",
            MatchEventType.MissedPenalty => "❌ Phạt đền hỏng",
            _ => ""
        };
        public string PlayerName { get; set; } = string.Empty;
        public string? SubstitutePlayerName { get; set; }
        public string? Description { get; set; }
        public int TeamId { get; set; }
    }

    public class MatchEventsViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime MatchDate { get; set; }
        public MatchStatus Status { get; set; }
        public List<MatchEventListItem> Events { get; set; } = new();

        public List<MatchEventListItem> HomeEvents => Events.Where(e => e.TeamId == HomeTeamId).ToList();
        public List<MatchEventListItem> AwayEvents => Events.Where(e => e.TeamId == AwayTeamId).ToList();
        public int HomeGoals => Events.Count(e => e.TeamId == HomeTeamId && e.EventType == MatchEventType.Goal)
                              + Events.Count(e => e.TeamId == AwayTeamId && e.EventType == MatchEventType.OwnGoal);
        public int AwayGoals => Events.Count(e => e.TeamId == AwayTeamId && e.EventType == MatchEventType.Goal)
                              + Events.Count(e => e.TeamId == HomeTeamId && e.EventType == MatchEventType.OwnGoal);
    }

    public class AddMatchEventViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn cầu thủ")]
        [Display(Name = "Cầu thủ")]
        public int PlayerId { get; set; }

        [Display(Name = "Cầu thủ vào sân (thay người)")]
        public int? SubstitutePlayerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sự kiện")]
        [Display(Name = "Loại sự kiện")]
        public MatchEventType EventType { get; set; }

        [Range(1, 120, ErrorMessage = "Phút phải từ 1 đến 120")]
        [Display(Name = "Phút")]
        public int? Minute { get; set; }

        [MaxLength(200)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public List<SelectListItem> HomePlayers { get; set; } = new();
        public List<SelectListItem> AwayPlayers { get; set; } = new();
        public List<SelectListItem> AllPlayers { get; set; } = new();
    }
}
