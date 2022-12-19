using CourseLibrary.API.Models.Users;
using FluentValidation;

namespace CourseLibrary.API.Validators.Users;

public class UserValidator : BaseValidator<User>
{
    public UserValidator(bool isNewEntity)
        : base(isNewEntity, false, true)
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
