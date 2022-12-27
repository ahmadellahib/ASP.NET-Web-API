using System.ComponentModel;

namespace CourseLibrary.API.Models.Users;

public class UserResourceParameters : ResourceParameters
{
    [DefaultValue(nameof(User.Id))]
    public override string OrderBy { get; set; } = nameof(User.Id);
}