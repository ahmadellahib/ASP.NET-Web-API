using CourseLibrary.API.Models.Courses;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker
{
    public DbSet<Course> Courses { get; set; }
}
