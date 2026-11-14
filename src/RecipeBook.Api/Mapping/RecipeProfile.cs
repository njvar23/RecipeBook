using AutoMapper;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        // ---------- Entity → DTO ----------

        CreateMap<Recipe, RecipeListDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.DisplayName))
            .ForMember(d => d.FavoritesCount, o => o.MapFrom(s => s.Favorites.Count))
            .ForMember(d => d.CommentsCount, o => o.MapFrom(s => s.Comments.Count));

        CreateMap<Recipe, RecipeDetailsDto>()
            .IncludeBase<Recipe, RecipeListDto>()
            // null when the author never filled in a chef profile (the 1:1 is optional)
            .ForMember(d => d.AuthorSpeciality, o => o.MapFrom(s =>
                s.Author.ChefProfile != null ? s.Author.ChefProfile.Speciality : null))
            .ForMember(d => d.Steps, o => o.MapFrom(s => s.Steps.OrderBy(x => x.Order)))
            .ForMember(d => d.Ingredients, o => o.MapFrom(s => s.RecipeIngredients))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments.OrderBy(c => c.CreatedAt)))
            // depends on who is asking, so RecipeService fills it in after mapping
            .ForMember(d => d.IsFavorite, o => o.Ignore());

        CreateMap<Step, StepDto>();

        // Flat ingredient line: the name comes from the nested Ingredient entity
        CreateMap<RecipeIngredient, RecipeIngredientDto>()
            .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient.Name));

        // ---------- Request → Entity (nested create) ----------

        CreateMap<StepRequest, Step>()
            .ForMember(d => d.Instruction, o => o.MapFrom(s => s.Instruction.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore());

        CreateMap<RecipeIngredientRequest, RecipeIngredient>()
            .ForMember(d => d.Unit, o => o.MapFrom(s => Enum.Parse<MeasurementUnit>(s.Unit, true)))
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.Ingredient, o => o.Ignore());

        // One map for the shared shape; Create/Update inherit it with IncludeBase.
        // Steps map by name (List<StepRequest> → ICollection<Step>);
        // Ingredients → RecipeIngredients is mapped explicitly.
        CreateMap<RecipeWriteRequest, Recipe>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title.Trim()))
            .ForMember(d => d.Difficulty, o => o.MapFrom(s => Enum.Parse<Difficulty>(s.Difficulty, true)))
            .ForMember(d => d.RecipeIngredients, o => o.MapFrom(s => s.Ingredients))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.AuthorId, o => o.Ignore())
            .ForMember(d => d.Author, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Comments, o => o.Ignore())
            .ForMember(d => d.Favorites, o => o.Ignore());

        CreateMap<CreateRecipeRequest, Recipe>().IncludeBase<RecipeWriteRequest, Recipe>();
        CreateMap<UpdateRecipeRequest, Recipe>().IncludeBase<RecipeWriteRequest, Recipe>();
    }
}
