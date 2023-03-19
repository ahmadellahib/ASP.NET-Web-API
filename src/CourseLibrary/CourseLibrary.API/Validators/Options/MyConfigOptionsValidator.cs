using CourseLibrary.API.Models.Options;
using FluentValidation;

namespace CourseLibrary.API.Validators.Options;

public class MyConfigOptionsValidator : AbstractValidator<MyConfigOptions>
{
    public MyConfigOptionsValidator()
    {
        // No validations required in this code sample
    }
}