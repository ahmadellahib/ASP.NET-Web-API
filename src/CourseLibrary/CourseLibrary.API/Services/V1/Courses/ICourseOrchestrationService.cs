using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Courses;

public interface ICourseOrchestrationService
{
    Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);

    Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken);

    Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);

    ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);

    IEnumerable<Course> RetrieveAllCourses();

    PagedList<Course> SearchCourses(CourseResourceParameters courseResourceParameters);
}
