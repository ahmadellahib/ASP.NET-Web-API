using CourseLibrary.API.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker
{
    internal DbSet<Course> Courses { get; set; }

    public async Task<Course> InsertCourseAsync(Course course, CancellationToken cancellationToken)
    {
        EntityEntry<Course> courseEntityEntry = await Courses.AddAsync(course, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return courseEntityEntry.Entity;
    }

    public async Task<Course> UpdateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        EntityEntry<Course> courseEntityEntry = Courses.Update(course);
        await SaveChangesAsync(cancellationToken);

        return courseEntityEntry.Entity;
    }

    public async Task<bool> DeleteCourseAsync(Course course, CancellationToken cancellationToken)
    {
        Courses.Remove(course);
        int result = await SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public IQueryable<Course> SelectAllCourses() =>
        Courses.AsQueryable();

    public async ValueTask<Course?> SelectCourseByIdAsync(Guid courseId, CancellationToken cancellationToken) =>
         await Courses.FindAsync(new object[] { courseId }, cancellationToken);
}
