using CourseLibrary.API.Models.Enums;

namespace CourseLibrary.API.Contracts.Users;

public class UserForUpdate
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
