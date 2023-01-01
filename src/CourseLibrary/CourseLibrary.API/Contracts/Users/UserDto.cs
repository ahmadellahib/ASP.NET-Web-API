using CourseLibrary.API.Models.Enums;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }
    public string ConcurrencyStamp { get; set; } = string.Empty;

    public static explicit operator UserDto(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Gender = user.Gender,
        DateOfBirth = user.DateOfBirth,
        DateOfDeath = user.DateOfDeath,
        ConcurrencyStamp = user.ConcurrencyStamp
    };
}
