using CourseLibrary.API.Models.Enums;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Contracts.Users;

public class UserForCreation
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset? DateOfDeath { get; set; }

    public static explicit operator User(UserForCreation userForCreation) => new()
    {
        FirstName = userForCreation.FirstName,
        LastName = userForCreation.LastName,
        Gender = userForCreation.Gender,
        DateOfBirth = userForCreation.DateOfBirth,
        DateOfDeath = userForCreation.DateOfDeath
    };
}