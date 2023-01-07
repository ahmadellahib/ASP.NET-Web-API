using CourseLibrary.API.Contracts.Authors;
using FluentValidation;

namespace CourseLibrary.API.Validators.Authors;

public sealed class AuthorForUpdateValidator : AbstractValidator<AuthorForUpdate>
{
    public AuthorForUpdateValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.MainCategoryId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.ConcurrencyStamp)
           .NotNull()
           .WithMessage(StaticData.ValidationMessages.CannotBeNull)
           .NotEmpty()
           .WithMessage(StaticData.ValidationMessages.CannotBeEmpty)
           .MaximumLength(255)
           .WithMessage(StaticData.ValidationMessages.MaxLength);
    }
}
