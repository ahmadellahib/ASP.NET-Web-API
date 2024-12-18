using CourseLibrary.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace CourseLibrary.Tests.Integration;

public class IntegrationTestFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    public IntegrationTestFactory()
    {
        _dbContainer = new MsSqlBuilder()
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            //Remove all background running services from doing undesired changes.
            services.RemoveAll(typeof(IHostedService));
        });

        SqlConnectionStringBuilder sqlConnectionStringBuilder = new(_dbContainer.GetConnectionString())
        {
            TrustServerCertificate = true,
            InitialCatalog = "CourseLibrary"
        };

        string connectionString = sqlConnectionStringBuilder.ConnectionString;

        ConfigurationBuilder configurationBuilder = new();
        Dictionary<string, string> dic = new()
        {
            {"ConnectionStrings:DefaultConnection", connectionString}
        };

        configurationBuilder.AddInMemoryCollection(dic);
        IConfigurationRoot Configuration = configurationBuilder.Build();
        builder.UseConfiguration(Configuration);
    }

    public async Task InitializeAsync() => await _dbContainer.StartAsync();

    public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}