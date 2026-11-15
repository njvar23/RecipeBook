using AutoMapper;
using RecipeBook.Api.DTOs.Comments;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.DisplayName));

        CreateMap<CreateCommentRequest, Comment>()
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Text.Trim()))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
