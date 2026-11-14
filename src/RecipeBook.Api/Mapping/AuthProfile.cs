using AutoMapper;
using RecipeBook.Api.DTOs.Auth;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        // Email is normalized so "Nino@Mail.ge" and "nino@mail.ge" are the same account.
        // PasswordHash / Role / CreatedAt are set by AuthService, never taken from the client.
        CreateMap<RegisterRequest, User>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Trim().ToLowerInvariant()))
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.DisplayName.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.ChefProfile, o => o.Ignore())
            .ForMember(d => d.Recipes, o => o.Ignore())
            .ForMember(d => d.Comments, o => o.Ignore())
            .ForMember(d => d.Favorites, o => o.Ignore());
    }
}
