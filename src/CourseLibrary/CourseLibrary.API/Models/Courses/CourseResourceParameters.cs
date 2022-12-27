using System.ComponentModel;

namespace CourseLibrary.API.Models.Courses;

public class CourseResourceParameters : ResourceParameters
{
    [DefaultValue(nameof(Course.Title))]
    public override string OrderBy { get; set; } = nameof(Course.Title);
}
