using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Auth;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class AuthService(
    AppDbContext db,
    IMapper mapper,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        // Email format + uniqueness were already checked by RegisterRequestValidator
        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.Hash(request.Password);
        user.Role = UserRole.User; // admins are only created by the seeder
        user.CreatedAt = DateTime.UtcNow;

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return mapper.Map<UserDto>(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        // Same message for "no such user" and "wrong password" — do not reveal which emails exist
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var (token, expiresAt) = tokenService.CreateToken(user);

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role.ToString()
        };
    }
}
