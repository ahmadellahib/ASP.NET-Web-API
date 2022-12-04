using CourseLibrary.API.Models.Authors;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker
{
    public DbSet<Author> Authors { get; set; }
}
