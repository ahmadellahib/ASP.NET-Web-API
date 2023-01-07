using CourseLibrary.API.Contracts.Users;
using FluentValidation;

namespace CourseLibrary.API.Validators.Users;

public sealed class UserForCreationValidator : AbstractValidator<UserForCreation>
{
    public UserForCreationValidator()
    {
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
    }
}
