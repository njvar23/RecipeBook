using AutoMapper;
using RecipeBook.Api.DTOs.Favorites;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        // Flattens Favorite → Recipe → Category / Author into one level
        CreateMap<Favorite, FavoriteDto>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Recipe.Title))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Recipe.Category.Name))
            .ForMember(d => d.Difficulty, o => o.MapFrom(s => s.Recipe.Difficulty))
            .ForMember(d => d.CookingMinutes, o => o.MapFrom(s => s.Recipe.CookingMinutes))
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Recipe.Author.DisplayName));
    }
}
