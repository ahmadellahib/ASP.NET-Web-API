﻿using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.Brokers.Storages;

internal partial interface IStorageBroker
{
    Task<Course> InsertCourseAsync(Course course, CancellationToken cancellationToken);
    Task<Course> UpdateCourseAsync(Course course, CancellationToken cancellationToken);
    Task<bool> DeleteCourseAsync(Course course, CancellationToken cancellationToken);
    Task<int> DeleteCoursesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);
    ValueTask<Course?> SelectCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
    IQueryable<Course> SelectAllCourses();
}
