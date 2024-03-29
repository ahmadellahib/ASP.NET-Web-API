﻿using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Contracts.Courses;

public class CourseDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AuthorFirstName { get; set; } = string.Empty;
    public string AuthorLastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator CourseDto(Course course) => new()
    {
        Id = course.Id,
        AuthorId = course.AuthorId,
        Title = course.Title,
        Description = course.Description,
        AuthorFirstName = course.Author.User.FirstName,
        AuthorLastName = course.Author.User.LastName,
        CreatedDate = course.CreatedDate,
        UpdatedDate = course.UpdatedDate,
        CreatedById = course.CreatedById,
        UpdatedById = course.UpdatedById,
        ConcurrencyStamp = course.ConcurrencyStamp
    };
}
