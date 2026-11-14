using AutoMapper;
using RecipeBook.Api.DTOs.ChefProfiles;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class ChefProfileProfile : Profile
{
    public ChefProfileProfile()
    {
        CreateMap<ChefProfile, ChefProfileDto>()
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))   // from the nested User
            .ForMember(d => d.RecipesCount, o => o.MapFrom(s => s.User.Recipes.Count)); // computed in SQL (COUNT)

        CreateMap<UpsertChefProfileRequest, ChefProfile>()
            .ForMember(d => d.Speciality, o => o.MapFrom(s => s.Speciality.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
