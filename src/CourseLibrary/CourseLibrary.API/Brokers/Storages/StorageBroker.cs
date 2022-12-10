using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker : DbContext, IStorageBroker
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public StorageBroker(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = _configuration.GetConnectionString(name: "DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("DefaultConnection is missing app configurations.");
        }

        optionsBuilder.UseSqlServer(connectionString);

        if (_env.IsDevelopment())
        {
            optionsBuilder.EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine);
        }
    }
}
