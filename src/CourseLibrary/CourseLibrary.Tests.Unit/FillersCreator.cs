using CourseLibrary.API.Models.Courses;
using Tynamix.ObjectFiller;

namespace CourseLibrary.Tests.Unit;

internal class FillersCreator
{
    #region Courses
    public static Filler<Course> CreateCourseFiller(DateTimeOffset dateTimeOffset)
    {
        Filler<Course> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(dateTimeOffset)
            .OnProperty(course => course.Author).IgnoreIt()
            .OnProperty(course => course.CreatedBy).IgnoreIt()
            .OnProperty(course => course.UpdatedBy).IgnoreIt();

        return filler;
    }
    #endregion
}