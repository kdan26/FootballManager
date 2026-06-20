using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IUserService
    {
        Task<List<UserListItem>> GetAllUsersAsync();
        Task<UserCreateViewModel> GetCreateFormAsync();
        Task<UserEditViewModel?> GetEditFormAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(UserCreateViewModel model);
        Task<(bool Success, string? Error)> UpdateAsync(UserEditViewModel model);
        Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, string newPassword);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<PlayerPortalViewModel?> GetPortalAsync(int userId);
    }
}
