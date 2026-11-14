using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Options;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class JwtTokenService(IOptions<JwtSettings> options) : ITokenService
{
    private readonly JwtSettings _jwt = options.Value;

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(AppClaimTypes.UserId, user.Id.ToString()),
            new Claim(AppClaimTypes.Email, user.Email),
            new Claim(AppClaimTypes.Role, user.Role.ToString()),
            new Claim(AppClaimTypes.Name, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(_jwt.ExpiresInHours);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
