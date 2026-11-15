using FluentValidation;
using RecipeBook.Api.DTOs.Comments;

namespace RecipeBook.Api.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text is required.")
            .Length(1, 500).WithMessage("Comment text must be between 1 and 500 characters.");
    }
}
