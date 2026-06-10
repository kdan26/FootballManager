using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IDrillService
    {
        Task<List<DrillListItem>> GetAllAsync(string? category = null, string? search = null);
        Task<DrillDetailViewModel?> GetDetailAsync(int id);
        Task<DrillFormViewModel?> GetFormAsync(int id);
        Task<int> CreateAsync(DrillFormViewModel model, int createdByUserId);
        Task UpdateAsync(DrillFormViewModel model);
        Task ToggleActiveAsync(int id);
        Task<List<DrillSelectItem>> GetSelectListAsync(string? category = null);
    }
}
