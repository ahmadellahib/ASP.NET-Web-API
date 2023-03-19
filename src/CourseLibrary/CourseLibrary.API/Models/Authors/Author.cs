using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Users;
using System.Diagnostics;

namespace CourseLibrary.API.Models.Authors;

[DebuggerDisplay("{User.FirstName,nq} {User.LastName,nq}")]
public class Author : IConcurrencyAware
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MainCategoryId { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public Category MainCategory { get; set; } = null!;
    public IEnumerable<Course>? Courses { get; set; }
}