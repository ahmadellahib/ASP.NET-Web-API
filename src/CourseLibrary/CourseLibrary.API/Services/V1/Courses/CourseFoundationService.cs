using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Services.V1.Courses;

public class CourseFoundationService : ICourseFoundationService
{
    public Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Course> RetrieveAllCourses()
    {
        throw new NotImplementedException();
    }

    public ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
