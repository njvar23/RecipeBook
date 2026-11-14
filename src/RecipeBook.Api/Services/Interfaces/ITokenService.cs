using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
