﻿namespace CourseLibrary.API.Models.Exceptions;

//public class ServiceException<T> : Exception, IServiceException where T : class
public class ServiceException : Exception, IServiceException
{
    public ServiceException(Exception innerException)
        : base($"Error occurred, contact support.", innerException) { }

    //public ServiceException(Exception innerException)
    //    : base($"{typeof(T).Name} error occurred, contact support.", innerException) { }
}
