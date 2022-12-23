using CourseLibrary.API.Models.Authors;
using FluentValidation;

namespace CourseLibrary.API.Validators.Authors;

public class AuthorValidator : BaseValidator<Author>
{
    public AuthorValidator(bool isNewEntity)
        : base(isNewEntity, false, true)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.MainCategory)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}
