using CourseLibrary.API.Models.Courses;
using FluentValidation;

namespace CourseLibrary.API.Validators.Courses;

internal sealed class CourseValidator : BaseValidator<Course>
{
    public CourseValidator()
        : this(false) { }

    public CourseValidator(bool isNewEntity)
        : base(isNewEntity, true, true)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);
    }
}