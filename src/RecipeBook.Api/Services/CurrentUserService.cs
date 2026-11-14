using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(AppClaimTypes.UserId)?.Value;
            return int.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedException("The access token does not contain a valid user id.");
        }
    }

    public bool IsAdmin =>
        httpContextAccessor.HttpContext?.User.IsInRole(nameof(UserRole.Admin)) ?? false;
}
