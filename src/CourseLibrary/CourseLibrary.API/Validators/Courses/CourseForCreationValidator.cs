using CourseLibrary.API.Contracts.Courses;
using FluentValidation;

namespace CourseLibrary.API.Validators.Courses;

internal sealed class CourseForCreationValidator : AbstractValidator<CourseForCreation>
{
    public CourseForCreationValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.CreatedById)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}