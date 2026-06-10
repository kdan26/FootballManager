using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class TeamListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HomeGround { get; set; } = string.Empty;
        public int MatchCount { get; set; }
    }

    public class TeamCreateViewModel
    {
        [Required(ErrorMessage = "Tên đội không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên đội không được vượt quá 100 ký tự")]
        [Display(Name = "Tên đội")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sân nhà không được để trống")]
        [MaxLength(200)]
        [Display(Name = "Sân nhà")]
        public string HomeGround { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }

    public class TeamEditViewModel : TeamCreateViewModel
    {
        public int Id { get; set; }
    }

    public class TeamDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HomeGround { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<MatchListItemViewModel> Matches { get; set; } = new();
    }
}
