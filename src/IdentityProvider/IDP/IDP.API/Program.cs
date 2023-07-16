using IDP.API.IoC;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.ConfigureSerilog()
    .ConfigureServices()
    .ConfigurePipeline();

app.Run();