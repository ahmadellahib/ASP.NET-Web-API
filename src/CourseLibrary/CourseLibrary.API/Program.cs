using CourseLibrary.API.IoC;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder
    .ConfigureSerilog()
    .ConfigureServices()
    .ConfigurePipeline();

bool useSqlServer = builder.Configuration.GetValue<bool>("MyConfig:UseSqlServer");

if (useSqlServer)
{
    await app.ResetDatabaseAsync();
}

app.Run();
