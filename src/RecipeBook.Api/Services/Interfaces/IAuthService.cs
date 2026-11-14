using RecipeBook.Api.DTOs.Auth;

namespace RecipeBook.Api.Services.Interfaces;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}
