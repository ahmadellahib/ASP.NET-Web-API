using CourseLibrary.API.Contracts.Categories;
using FluentValidation;

namespace CourseLibrary.API.Validators.Categories;

public sealed class CategoryForUpdateValidator : AbstractValidator<CategoryForUpdate>
{
    public CategoryForUpdateValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.UpdatedById)
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
