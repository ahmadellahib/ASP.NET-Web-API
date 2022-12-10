using CourseLibrary.API.Models.Courses;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker
{
    internal DbSet<Course> Courses { get; set; }

    public Task<Course> InsertCourseAsync(Course course, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Course> UpdateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCourseAsync(Course course, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Course> SelectAllCourses()
    {
        throw new NotImplementedException();
    }

    public ValueTask<Course?> SelectCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
