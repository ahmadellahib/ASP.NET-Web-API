using CourseLibrary.API.Models.Enums;

namespace CourseLibrary.API.Contracts.Users;

public class UserForCreation
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
}
