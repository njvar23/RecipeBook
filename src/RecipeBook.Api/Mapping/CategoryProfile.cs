using AutoMapper;
using RecipeBook.Api.DTOs.Categories;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.RecipesCount, o => o.MapFrom(s => s.Recipes.Count));

        CreateMap<CreateCategoryRequest, Category>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Recipes, o => o.Ignore());
    }
}
