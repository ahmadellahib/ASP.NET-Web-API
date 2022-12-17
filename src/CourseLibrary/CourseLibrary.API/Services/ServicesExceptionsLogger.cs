using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CourseLibrary.API.Services;

public class ServicesExceptionsLogger<T> : IServicesExceptionsLogger<T> where T : class
{
    private readonly ILoggingBroker<T> _loggingBroker;
    private static readonly Regex UniqueConstraintRegex = new("'([a-zA-Z0-9]*)_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)'", RegexOptions.Compiled);
    private static readonly Regex PrimaryKeyConstraintRegex = new("'PK_([a-zA-Z0-9]*)'", RegexOptions.Compiled);
    private static readonly Regex ForeignKeyConstraintRegex = new("FK_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)", RegexOptions.Compiled);

    public ServicesExceptionsLogger(ILoggingBroker<T> loggingBroker)
    {
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    public CancellationException CreateAndLogCancellationException(Exception exception)
    {
        CancellationException cancellationException = new();
        _loggingBroker.LogWarning(exception.Message);

        return cancellationException;
    }

    public DependencyException<T> CreateAndLogCriticalConflictException(Exception exception)
    {
        DependencyException<T> dependencyException = new(exception);
        _loggingBroker.LogCritical(dependencyException);

        return dependencyException;
    }

    public DependencyException<T> CreateAndLogCriticalDependencyException(Exception exception)
    {
        DependencyException<T> dependencyException = new(exception);
        _loggingBroker.LogCritical(dependencyException);

        return dependencyException;
    }

    public DependencyException<T> CreateAndLogDependencyException(Exception exception)
    {
        DependencyException<T> dependencyException = new(exception);

        if (exception is DbUpdateException && exception.InnerException != null && exception.InnerException is SqlException sqlException)
        {
            switch (sqlException!.Number)
            {
                case 2627:  // PRIMARY KEY constraint error => DuplicateKeyException
                case 547:   // Constraint check violation => ForeignKeyConstraintConflictException
                case 2601:  // Duplicated key row error
                            // Constraint violation exception => DuplicateKeyWithUniqueIndexException
                    string extraMessage = GetSqlExceptionExtraMessage(sqlException);
                    dependencyException = new DependencyException<T>(new DbConflictException(sqlException), extraMessage);
                    break;
            }
        }

        _loggingBroker.LogError(dependencyException);

        return dependencyException;
    }

    public ServiceException<T> CreateAndLogServiceException(Exception exception)
    {
        ServiceException<T> serviceException = new(exception);
        _loggingBroker.LogError(serviceException);

        return serviceException;
    }

    public ValidationException CreateAndLogValidationException(Exception exception)
    {
        ValidationException validationException = new(exception);
        _loggingBroker.LogError(validationException);

        return validationException;
    }

    public ValidationException CreateValidationException(Exception exception) =>
         new(exception);

    private string GetSqlExceptionExtraMessage(SqlException sqlException)
    {
        switch (sqlException.Number)
        {
            case 547:
                return GetForeignKeyConstraintMessage(sqlException.Message);
            case 2601:
                return GetUniqueConstraintMessage(sqlException.Message);
            case 2627:
                return GetPrimaryKeyConstraintMessage(sqlException.Message);
            default:
                return string.Empty;
        }
    }

    private string GetForeignKeyConstraintMessage(string sqlExceptionMessage)
    {
        MatchCollection matches = ForeignKeyConstraintRegex.Matches(sqlExceptionMessage);

        if (matches.Count == 0)
            return "Reference for a non existing entity.";

        return $"{matches[0].Groups[3].Value} references a non existing entity.";
    }

    private string GetPrimaryKeyConstraintMessage(string sqlExceptionMessage)
    {
        string keyValue = "";
        MatchCollection matches = PrimaryKeyConstraintRegex.Matches(sqlExceptionMessage);

        if (matches.Count == 0)
            return "Primary key must be unique.";

        int openingBracketIndex = sqlExceptionMessage.IndexOf("(");
        int ClosingBracketIndex = sqlExceptionMessage.IndexOf(")");

        if (openingBracketIndex > 0 && ClosingBracketIndex > 0)
            keyValue = $"{sqlExceptionMessage.Substring(openingBracketIndex, ClosingBracketIndex - openingBracketIndex + 1)}";

        return $"Cannot have a duplicate primary key{keyValue} in {matches[0].Groups[1].Value}.";
    }

    private string GetUniqueConstraintMessage(string sqlExceptionMessage)
    {
        string keyValue = "";
        MatchCollection matches = UniqueConstraintRegex.Matches(sqlExceptionMessage);

        if (matches.Count == 0)
            return "A key must be unique.";

        int openingBracketIndex = sqlExceptionMessage.IndexOf("(");
        int ClosingBracketIndex = sqlExceptionMessage.IndexOf(")");

        if (openingBracketIndex > 0 && ClosingBracketIndex > 0)
            keyValue = $"{sqlExceptionMessage.Substring(openingBracketIndex, ClosingBracketIndex - openingBracketIndex + 1)}";

        return $"Cannot have a duplicate {matches[0].Groups[3].Value}{keyValue} in {matches[0].Groups[2].Value}.";
    }
}
