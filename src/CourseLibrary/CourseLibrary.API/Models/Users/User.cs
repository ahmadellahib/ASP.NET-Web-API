﻿using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Enums;
using System.Diagnostics;

namespace CourseLibrary.API.Models.Users;

[DebuggerDisplay("{FirstName,nq} {LastName,nq}")]
public class User : IConcurrencyAware
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public IEnumerable<Course>? CreatedCourses { get; set; }
    public IEnumerable<Course>? UpdatedCourses { get; set; }
    public IEnumerable<Category>? CreatedCategories { get; set; }
    public IEnumerable<Category>? UpdatedCategories { get; set; }
}
