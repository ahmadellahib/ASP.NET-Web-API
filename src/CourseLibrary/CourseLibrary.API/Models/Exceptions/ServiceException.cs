﻿namespace CourseLibrary.API.Models.Exceptions;

public class ServiceException<T> : Exception, IServiceException where T : class
{
    public ServiceException(Exception innerException)
        : base($"{typeof(T).Name} error occurred, contact support.", innerException) { }
}
