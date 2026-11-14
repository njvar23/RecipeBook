using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.ChefProfiles;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class ChefProfileService(
    AppDbContext db,
    IMapper mapper,
    ICurrentUserService currentUser) : IChefProfileService
{
    public async Task<ChefProfileDto> GetMineAsync(CancellationToken ct)
    {
        return await FindDtoByUserIdAsync(currentUser.UserId, ct)
               ?? throw new NotFoundException("You have not created a chef profile yet.");
    }

    public async Task<(ChefProfileDto Profile, bool Created)> UpsertMineAsync(
        UpsertChefProfileRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var profile = await db.ChefProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        var created = profile is null;

        if (profile is null)
        {
            profile = mapper.Map<ChefProfile>(request);
            profile.UserId = userId;
            db.ChefProfiles.Add(profile);
        }
        else
        {
            mapper.Map(request, profile); // copy the new values onto the tracked entity
        }

        await db.SaveChangesAsync(ct);

        var dto = await FindDtoByUserIdAsync(userId, ct);
        return (dto!, created);
    }

    public async Task<ChefProfileDto> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(u => u.Id == userId, ct))
            throw NotFoundException.For("User", userId);

        return await FindDtoByUserIdAsync(userId, ct)
               ?? throw new NotFoundException($"User {userId} does not have a chef profile.");
    }

    private Task<ChefProfileDto?> FindDtoByUserIdAsync(int userId, CancellationToken ct) =>
        db.ChefProfiles
            .Where(p => p.UserId == userId)
            .ProjectTo<ChefProfileDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
}
