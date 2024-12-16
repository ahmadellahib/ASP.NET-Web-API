namespace CourseLibrary.API.Models.Exceptions;

public class NotFoundEntityException : Exception
{
    public NotFoundEntityException(Type T, params Guid[] ids)
        : base(GenerateBaseMessage(T, ids)) { }

    public NotFoundEntityException(Type T, params int[] ids)
        : base(GenerateBaseMessage(T, ids)) { }

    public NotFoundEntityException(string message)
        : base(message) { }

    private static string GenerateBaseMessage(Type T, params Guid[] ids)
    {
        string idsString = ConvertParamsToString(ids);

        return $"The requested resource {T.Name} with id: {idsString} could not be found.";
    }

    private static string GenerateBaseMessage(Type T, params int[] ids)
    {
        string idsString = ConvertParamsToString(ids);

        return $"The requested resource {T.Name} with id: {idsString} could not be found.";
    }

    private static string ConvertParamsToString(params Guid[] ids)
    {
        bool addedId = false;
        string idsString = string.Empty;

        foreach (Guid id in ids)
        {
            if (addedId)
            {
                idsString += ", ";
            }

            idsString += id;
            addedId = true;
        }

        return idsString;
    }

    private static string ConvertParamsToString(params int[] ids)
    {
        bool addedId = false;
        string idsString = string.Empty;

        foreach (int id in ids)
        {
            if (addedId)
            {
                idsString += ", ";
            }

            idsString += id;
            addedId = true;
        }

        return idsString;
    }
}