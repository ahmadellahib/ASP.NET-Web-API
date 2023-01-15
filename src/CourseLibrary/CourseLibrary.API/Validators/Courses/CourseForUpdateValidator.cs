using CourseLibrary.API.Contracts.Courses;
using FluentValidation;

namespace CourseLibrary.API.Validators.Courses;

public sealed class CourseForUpdateValidator : AbstractValidator<CourseForUpdate>
{
    public CourseForUpdateValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Title)
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