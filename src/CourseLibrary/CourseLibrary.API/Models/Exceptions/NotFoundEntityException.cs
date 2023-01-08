namespace CourseLibrary.API.Models.Exceptions;

public class NotFoundEntityException<T> : Exception
{
    public NotFoundEntityException(params Guid[] ids)
        : base(GenerateBaseMessage(ids)) { }

    public NotFoundEntityException(params int[] ids)
        : base(GenerateBaseMessage(ids)) { }

    public NotFoundEntityException(string message)
        : base(message) { }

    private static string GenerateBaseMessage(params Guid[] ids)
    {
        string idsString = ConvertParamsToString(ids);

        return $"The requested resource {typeof(T).Name} with id: {idsString} could not be found.";
    }

    private static string GenerateBaseMessage(params int[] ids)
    {
        string idsString = ConvertParamsToString(ids);

        return $"The requested resource {typeof(T).Name} with id: {idsString} could not be found.";
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