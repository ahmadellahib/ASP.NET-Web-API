namespace CourseLibrary.API;

internal static class StaticData
{
    public class ValidationMessages
    {
        public const string InvalidValue = "Is not a valid value.";
        public const string InvalidEmail = "Is not a valid email address.";
        public const string CannotBeNull = "Cannot be a null value.";
        public const string MustBeNull = "Must be null";
        public const string EntityIsNull = "Ensure a model is supplied.";
        public const string CannotBeEmpty = "Cannot be empty.";
        public const string MinLength = "Minimum length must be greater than or equal to {MinLength}";
        public const string MaxLength = "Maximum length must be less than or equal to {MaxLength}";
        public const string AuditingFieldsUsersIdsAreDifferentOnCreatingEntity = "Auditing users Ids are different on creating new entity.";
        public const string AuditingFieldsDatesAreDifferentOnCreatingEntity = "Auditing dates are different on creating new entity.";
        public const string AuditingFieldsCreatedDateIsNotRecentOnCreatingEntity = "Auditing create date is not recent on creating new entity.";
        public const string AuditingFieldsDatesAreSameOnUpdateEntity = "Auditing dates are same on updating entity.";
        public const string AuditingFieldsUpdateDateIsNotRecent = "Auditing update date is not recent";
        public const string MustBeEmptyString = "Must be empty string";
        public const string InvalidEnumValue = "Invalid enum value";
    }

    public class WarningMessages
    {
        public const string NoEntitiesFoundInStorage = "No entities found in storage.";
    }

    public static class ExceptionMessages
    {
        public static string NoRowsWasEffectedByDeleting(string className, object id)
        {
            return $"No rows was effected while deleting a {className} with id {id}";
        }

        public static string NoRowsWasEffectedByDeleting(string className, object id1, object id2)
        {
            return $"No rows was effected while deleting a {className} with composite id ({id1}, {id2})";
        }
    }
}