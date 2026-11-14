using RecipeBook.Api.DTOs.ChefProfiles;

namespace RecipeBook.Api.Services.Interfaces;

public interface IChefProfileService
{
    Task<ChefProfileDto> GetMineAsync(CancellationToken ct);

    /// <returns>The profile and whether it was newly created (201) or updated (200).</returns>
    Task<(ChefProfileDto Profile, bool Created)> UpsertMineAsync(UpsertChefProfileRequest request, CancellationToken ct);

    Task<ChefProfileDto> GetByUserIdAsync(int userId, CancellationToken ct);
}
