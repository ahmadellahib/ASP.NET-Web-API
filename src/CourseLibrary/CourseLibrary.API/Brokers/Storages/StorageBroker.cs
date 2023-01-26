using CourseLibrary.API.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace CourseLibrary.API.Brokers.Storages;

internal sealed partial class StorageBroker : DbContext, IStorageBroker
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly MyConfigOptions _myConfigOptions;

    public StorageBroker(IWebHostEnvironment env, IConfiguration configuration, IOptions<MyConfigOptions> myConfigOptions)
    {
        _myConfigOptions = myConfigOptions.Value;
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
        if (_myConfigOptions.UseSqlServer)
        {
            string? connectionString = _configuration.GetConnectionString(name: "DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("DefaultConnection is missing app configurations.");
            }

            optionsBuilder.UseSqlServer(connectionString,
                providerOptions =>
                {
                    providerOptions.EnableRetryOnFailure();
                })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
        else
        {
            optionsBuilder.UseInMemoryDatabase("CourseLibraryDb")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        if (_env.IsDevelopment())
        {
            //optionsBuilder.EnableDetailedErrors()
            //    .EnableSensitiveDataLogging()
            //    .LogTo(Console.WriteLine);
        }
    }
}
