using CourseLibrary.API.Models.Authors;
using FluentValidation;

namespace CourseLibrary.API.Validators.Authors;

internal sealed class AuthorValidator : BaseValidator<Author>
{
    public AuthorValidator()
        : this(false) { }

    public AuthorValidator(bool isNewEntity)
        : base(isNewEntity, false, true)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.MainCategoryId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}
