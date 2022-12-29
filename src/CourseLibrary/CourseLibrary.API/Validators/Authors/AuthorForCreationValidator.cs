using CourseLibrary.API.Contracts.Authors;
using FluentValidation;

namespace CourseLibrary.API.Validators.Authors;

internal sealed class AuthorForCreationValidator : AbstractValidator<AuthorForCreation>
{
    public AuthorForCreationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.MainCategoryId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}