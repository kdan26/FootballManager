using FootballManager.Models;
using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IPlayerService
    {
        Task<List<PlayerListItemViewModel>> GetPlayersByTeamAsync(int teamId);
        Task<PlayerDetailsViewModel?> GetPlayerDetailsAsync(int id);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreatePlayerAsync(PlayerCreateViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdatePlayerAsync(int id, PlayerEditViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeletePlayerAsync(int id);

        // Sức khỏe
        Task UpdateHealthAsync(int playerId, PlayerHealthStatus status, string? note, DateTime? expectedReturn);

        // Chiến thuật đội
        Task SaveTacticsAsync(int teamId, string formation, string? notes);
        Task SaveTacticsBoardAsync(int teamId, string formation, string? notes,
            string? positionsJson, string? arrowsJson);
        Task<TeamTactics?> GetTacticsAsync(int teamId);
    }
}
