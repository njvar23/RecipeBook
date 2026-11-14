using AutoMapper;
using RecipeBook.Api.DTOs.Ingredients;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>();

        CreateMap<CreateIngredientRequest, Ingredient>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RecipeIngredients, o => o.Ignore());
    }
}
