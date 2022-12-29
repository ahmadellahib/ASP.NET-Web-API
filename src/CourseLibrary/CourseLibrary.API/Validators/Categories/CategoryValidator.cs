using CourseLibrary.API.Models.Categories;
using FluentValidation;

namespace CourseLibrary.API.Validators.Categories;

internal sealed class CategoryValidator : BaseValidator<Category>
{
    public CategoryValidator()
        : this(false) { }

    public CategoryValidator(bool isNewEntity)
        : base(isNewEntity, true, true)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}
