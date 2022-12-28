using CourseLibrary.API.Contracts.Users;
using FluentValidation;

namespace CourseLibrary.API.Validators.Users;

internal sealed class UserForUpdateValidator : AbstractValidator<UserForUpdate>
{
    public UserForUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage(StaticData.ValidationMessages.InvalidEnumValue);

        RuleFor(x => x.DateOfBirth)
            .NotNull()
            .WithMessage(StaticData.ValidationMessages.CannotBeNull);

        RuleFor(x => x.ConcurrencyStamp)
            .NotNull()
            .WithMessage(StaticData.ValidationMessages.CannotBeNull)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty)
            .MaximumLength(255)
            .WithMessage(StaticData.ValidationMessages.MaxLength);
    }
}