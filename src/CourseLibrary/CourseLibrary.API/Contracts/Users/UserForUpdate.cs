using CourseLibrary.API.Models.Enums;
using CourseLibrary.API.Models.Users;

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

    public static explicit operator User(UserForUpdate userForUpdate) => new()
    {
        Id = userForUpdate.Id,
        FirstName = userForUpdate.FirstName,
        LastName = userForUpdate.LastName,
        Gender = userForUpdate.Gender,
        DateOfBirth = userForUpdate.DateOfBirth,
        DateOfDeath = userForUpdate.DateOfDeath,
        ConcurrencyStamp = userForUpdate.ConcurrencyStamp
    };
}
