namespace CourseLibrary.Tests.Unit;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Reflection;

internal class SqlExceptionBuilder
{
    private int errorNumber;
    private string errorMessage = String.Empty;

    public SqlException Build()
    {
        SqlError? error = this.CreateError();

        if (error is null)
        {
            throw new("Exception creating error");
        }

        SqlErrorCollection errorCollection = this.CreateErrorCollection(error);
        SqlException exception = this.CreateException(errorCollection);

        return exception;
    }

    public SqlExceptionBuilder WithErrorNumber(int number)
    {
        this.errorNumber = number;
        return this;
    }

    public SqlExceptionBuilder WithErrorMessage(string message)
    {
        this.errorMessage = message;
        return this;
    }

    private SqlError? CreateError()
    {
        // Create instance via reflection...
        ConstructorInfo[] ctors = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        ConstructorInfo? firstSqlErrorCtor = ctors.FirstOrDefault(
            ctor =>
            ctor.GetParameters().Length == 8); // Need a specific constructor!

        if (firstSqlErrorCtor is null)
        {
            return null;
        }

        SqlError? error = firstSqlErrorCtor.Invoke(
            new object[]
            {
            this.errorNumber,
            new byte(),
            new byte(),
            string.Empty,
            string.Empty,
            string.Empty,
            new int(),
            null
            }) as SqlError;

        return error;
    }

    private SqlErrorCollection CreateErrorCollection(SqlError error)
    {
        // Create instance via reflection...
        ConstructorInfo sqlErrorCollectionCtor = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
        SqlErrorCollection? errorCollection = sqlErrorCollectionCtor.Invoke(new object[] { }) as SqlErrorCollection;

        // Add error...
        MethodInfo? methodInfo = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo is null)
        {
            throw new("methodInfo is null");
        }

        methodInfo.Invoke(errorCollection, new object[] { error });

        if (errorCollection is null)
        {
            throw new("errorCollection is null");
        }

        return errorCollection;
    }

    private SqlException CreateException(SqlErrorCollection errorCollection)
    {
        // Create instance via reflection...
        ConstructorInfo ctor = typeof(SqlException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

        SqlException? sqlException = ctor.Invoke(
            new object[]
            { 
            // With message and error collection...
            this.errorMessage,
            errorCollection,
            null,
            Guid.NewGuid()
            }) as SqlException;

        if (sqlException is null)
        {
            throw new("sqlException is null");
        }

        return sqlException;
    }
}
