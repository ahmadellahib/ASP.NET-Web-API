using CourseLibrary.API.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Globalization;

namespace CourseLibrary.API.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    private readonly bool _isNewEntity;
    private readonly bool _isAuditable;
    private readonly bool _isConcurrencyAware;

    public BaseValidator()
        : this(false, false, false) { }

    public BaseValidator(bool isNewEntity)
        : this(isNewEntity, false, false) { }

    public BaseValidator(bool isNewEntity, bool isAuditable, bool isConcurrencyAware)
    {
        _isNewEntity = isNewEntity;
        _isAuditable = isAuditable;
        _isConcurrencyAware = isConcurrencyAware;

        if (_isConcurrencyAware)
        {
            if (!_isNewEntity)
            {
                RuleFor(x => ((IConcurrencyAware)x!).ConcurrencyStamp)
                   .NotNull()
                   .WithMessage(StaticData.ValidationMessages.CannotBeNull)
                   .NotEmpty()
                   .WithMessage(StaticData.ValidationMessages.CannotBeEmpty)
                   .MaximumLength(255)
                   .WithMessage(StaticData.ValidationMessages.MaxLength);
            }
        }

        if (_isAuditable)
        {
            RuleFor(x => ((IAuditable)x!).CreatedById)
                .NotEmpty()
                .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

            RuleFor(x => ((IAuditable)x!).UpdatedById)
                .NotEmpty()
                .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

            RuleFor(x => ((IAuditable)x!).CreatedDate)
                .NotEmpty()
                .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

            RuleFor(x => ((IAuditable)x!).UpdatedDate)
                .NotEmpty()
                .WithMessage(StaticData.ValidationMessages.CannotBeEmpty);

            if (_isNewEntity)
            {
                RuleFor(x => (IAuditable)x!)
                    .Custom((x, context) =>
                    {
                        if (x.CreatedById != x.UpdatedById)
                        {
                            context.AddFailure("Auditing fields", StaticData.ValidationMessages.AuditingFieldsUsersIdsAreDifferentOnCreatingEntity);
                        }

                        if (x.CreatedDate != x.UpdatedDate)
                        {
                            context.AddFailure("Auditing fields", StaticData.ValidationMessages.AuditingFieldsDatesAreDifferentOnCreatingEntity);
                        }

                        if (!IsDateRecent(x.CreatedDate))
                        {
                            context.AddFailure("Auditing fields", StaticData.ValidationMessages.AuditingFieldsCreatedDateIsNotRecentOnCreatingEntity);
                        }
                    });
            }
            else
            {
                RuleFor(x => (IAuditable)x!)
                    .Custom((x, context) =>
                    {
                        if (x.CreatedDate == x.UpdatedDate)
                        {
                            context.AddFailure("Auditing fields", StaticData.ValidationMessages.AuditingFieldsDatesAreSameOnUpdateEntity);
                        }

                        if (!IsDateRecent(x.UpdatedDate))
                        {
                            context.AddFailure("Auditing fields", StaticData.ValidationMessages.AuditingFieldsUpdateDateIsNotRecent);
                        }
                    });
            }
        }
    }

    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure($"Entity is null.", StaticData.ValidationMessages.EntityIsNull));
            return false;
        }
        return true;
    }

    protected bool IsValidateGuid(Guid guid)
    {
        return guid == Guid.Empty;
    }

    protected bool IsValidDomain(string domain)
    {
        return Uri.CheckHostName(domain) != UriHostNameType.Unknown;
    }

    protected bool IsValidCultureName(string cultureName)
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Any(culture => string.Equals(culture.Name, cultureName, StringComparison.InvariantCultureIgnoreCase));
    }

    protected bool IsDateRecent(DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        int oneMinute = 1;
        TimeSpan difference = now.Subtract(dateTimeOffset);
        return Math.Abs(difference.TotalMinutes) < oneMinute;
    }
}
