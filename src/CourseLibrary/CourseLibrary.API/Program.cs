using CourseLibrary.API.IoC;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder
    .ConfigureSerilog()
    .ConfigureServices()
    .ConfigurePipeline();

await app.ResetDatabaseAsync();

app.Run();
