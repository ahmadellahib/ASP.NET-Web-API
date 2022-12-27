using CourseLibrary.API.Contracts.Authors;
using FluentValidation;

namespace CourseLibrary.API.Validators.Authors;

public class AuthorForCreationValidator : AbstractValidator<AuthorForCreation>
{
    public AuthorForCreationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.MainCategory)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}