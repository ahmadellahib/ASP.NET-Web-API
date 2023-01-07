using CourseLibrary.API.Contracts.Categories;
using FluentValidation;

namespace CourseLibrary.API.Validators.Categories;

public sealed class CategoryForCreationValidator : AbstractValidator<CategoryForCreation>
{
    public CategoryForCreationValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.CreatedById)
          .NotEmpty()
          .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}
