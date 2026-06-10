using FootballManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class MatchListItemViewModel
    {
        public int Id { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string? Venue { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
    }

    public class MatchCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn đội nhà")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đội nhà")]
        [Display(Name = "Đội nhà")]
        public int HomeTeamId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đội khách")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đội khách")]
        [Display(Name = "Đội khách")]
        public int AwayTeamId { get; set; }

        [Required(ErrorMessage = "Ngày thi đấu không được để trống")]
        [Display(Name = "Ngày thi đấu")]
        public DateTime MatchDate { get; set; } = DateTime.Now.AddDays(1);

        [MaxLength(200)]
        [Display(Name = "Địa điểm")]
        public string? Venue { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        public List<SelectListItem> Teams { get; set; } = new();
    }

    public class MatchResultViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số bàn thắng không được âm")]
        [Display(Name = "Bàn thắng đội nhà")]
        public int HomeScore { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số bàn thắng không được âm")]
        [Display(Name = "Bàn thắng đội khách")]
        public int AwayScore { get; set; }
    }

    public class MatchDetailsViewModel
    {
        public int Id { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string? Venue { get; set; }
        public string? Notes { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}
