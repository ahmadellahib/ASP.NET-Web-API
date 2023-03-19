using CourseLibrary.API.Models.Users;
using System.Diagnostics;

namespace CourseLibrary.API.Models.Categories;

[DebuggerDisplay("{Name,nq}")]
public class Category : IConcurrencyAware, IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public Guid UpdatedById { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public User CreatedBy { get; set; } = null!;
    public User UpdatedBy { get; set; } = null!;
}