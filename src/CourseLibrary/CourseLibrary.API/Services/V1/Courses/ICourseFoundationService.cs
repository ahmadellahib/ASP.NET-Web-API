﻿using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Services.V1.Courses;

public interface ICourseFoundationService
{
    Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);

    Task<Course> ModifyCourseAsync(Course course, CancellationToken cancellationToken);

    Task RemoveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task RemoveCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);

    ValueTask<Course> RetrieveCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);

    IQueryable<Course> RetrieveAllCourses();
}
