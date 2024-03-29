﻿using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Contracts.Authors;

public class AuthorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MainCategory { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator AuthorDto(Author author) => new()
    {
        Id = author.Id,
        UserId = author.UserId,
        MainCategoryId = author.MainCategoryId,
        FirstName = author.User.FirstName,
        LastName = author.User.LastName,
        MainCategory = author.MainCategory.Name,
        ConcurrencyStamp = author.ConcurrencyStamp
    };
}