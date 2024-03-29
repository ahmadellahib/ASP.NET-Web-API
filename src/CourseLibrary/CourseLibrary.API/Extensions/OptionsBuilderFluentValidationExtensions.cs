﻿using FluentValidation;
using Microsoft.Extensions.Options;

namespace CourseLibrary.API.Extensions;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(s => new FluentValidationOptions<TOptions>(optionsBuilder.Name, s.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }

    private class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;
        public string Name { get; }

        public FluentValidationOptions(string name, IValidator<TOptions> validator)
        {
            Name = name;
            _validator = validator;
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            if (Name != null && Name != name)
            {
                return ValidateOptionsResult.Skip;
            }

            ArgumentNullException.ThrowIfNull(options);

            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(options);

            if (validationResult.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            IEnumerable<string> errors = validationResult.Errors.Select(x => $"Options validation failed for [{x.PropertyName}] with error: [{x.ErrorMessage}]");
            return ValidateOptionsResult.Fail(errors);
        }
    }
}