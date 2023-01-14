using Bogus;
using CategoryLibrary.API.Services.V1.Categories;
using CourseLibrary.API.Brokers.Caches;
using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Extensions;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Enums;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Services;
using CourseLibrary.API.Services.V1.Authors;
using CourseLibrary.API.Services.V1.Categories;
using CourseLibrary.API.Services.V1.Courses;
using CourseLibrary.API.Services.V1.PropertyMappings;
using CourseLibrary.API.Services.V1.Users;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CourseLibrary.API.IoC;

internal static class StartupHelperExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterDependencies()
            .AddMemoryCache()
            .RegisterDbContext()
            .RegisterApiVersioning()
            .AddEndpointsApiExplorer()
            .RegisterSwagger()
            .RegisterFluentValidation()
            .AddControllers();

        return builder.Build();
    }

    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom
            .Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(
                        "An unexpected fault happened. Try again later.");
                });
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static async Task ResetDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        try
        {
            StorageBroker? storageBroker = scope.ServiceProvider.GetService<StorageBroker>();

            if (storageBroker != null)
            {
                if (app.Environment.IsDevelopment())
                {
                    await storageBroker.Database.EnsureDeletedAsync();
                }

                IEnumerable<string> pendingMigrations = await storageBroker.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    await storageBroker.Database.MigrateAsync();
                }

                await SeedData(storageBroker);
            }
        }
        catch (Exception ex)
        {
            Microsoft.Extensions.Logging.ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private static async Task SeedData(StorageBroker storageBroker)
    {
        if (!storageBroker.Users.Any())
        {
            //Set the randomizer seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);

            Faker<User> testUsers = new Faker<User>()
                .StrictMode(false)
                .RuleFor(x => x.Gender, f => f.PickRandom<Gender>())
                .RuleFor(x => x.FirstName, (f, x) => f.Name.FirstName(ReturnGenderType(x.Gender)))
                .RuleFor(x => x.LastName, (f, x) => f.Name.LastName(ReturnGenderType(x.Gender)))
                .RuleFor(x => x.DateOfBirth, f => f.Date.PastOffset(30, DateTime.Now.AddYears(-30)))
                .RuleFor(x => x.DateOfDeath, f => null)
                .RuleFor(x => x.ConcurrencyStamp, f => f.Random.Guid().ToString());

            List<User> users = testUsers.Generate(30);
            await storageBroker.Users.AddRangeAsync(users);
            await storageBroker.SaveChangesAsync();

            int index = 0;
            Faker<Category> testCategories = new Faker<Category>()
                .StrictMode(false)
                .RuleFor(x => x.Name, f => f.CourseCategory().Name(index++))
                .RuleFor(x => x.CreatedDate, f => DateTimeOffset.UtcNow.AddDays(f.Random.Number(-100, -1)))
                .RuleFor(x => x.UpdatedDate, f => DateTimeOffset.UtcNow.AddDays(f.Random.Number(1, 100)))
                .RuleFor(x => x.CreatedById, f => f.PickRandom(users).Id)
                .RuleFor(x => x.UpdatedById, f => f.PickRandom(users).Id)
                .RuleFor(x => x.ConcurrencyStamp, f => f.Random.Guid().ToString());

            List<Category> categories = testCategories.Generate(11);
            await storageBroker.Categories.AddRangeAsync(categories);
            await storageBroker.SaveChangesAsync();

            Faker<Author> testAuthors = new Faker<Author>()
                .StrictMode(false)
                .RuleFor(x => x.UserId, f => f.PickRandom(users).Id)
                .RuleFor(x => x.MainCategoryId, f => f.PickRandom(categories).Id)
                .RuleFor(x => x.ConcurrencyStamp, f => f.Random.Guid().ToString());

            List<Author> authors = testAuthors.Generate(200)
                .GroupBy(a => a.UserId)
                .Select(a => a.First())
                .Take(20)
                .ToList();

            await storageBroker.Authors.AddRangeAsync(authors);
            await storageBroker.SaveChangesAsync();

            Faker<Course> testCourses = new Faker<Course>()
                .StrictMode(false)
                .RuleForType(typeof(string), f => f.Lorem.Word())
                .RuleFor(x => x.AuthorId, f => f.PickRandom(authors).Id)
                .RuleFor(x => x.Title, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Paragraph())
                .RuleFor(x => x.CreatedDate, f => DateTimeOffset.UtcNow.AddDays(f.Random.Number(-100, -1)))
                .RuleFor(x => x.UpdatedDate, f => DateTimeOffset.UtcNow.AddDays(f.Random.Number(1, 100)))
                .RuleFor(x => x.CreatedById, f => f.PickRandom(users).Id)
                .RuleFor(x => x.UpdatedById, f => f.PickRandom(users).Id)
                .RuleFor(x => x.ConcurrencyStamp, f => f.Random.Guid().ToString());

            await storageBroker.Courses.AddRangeAsync(testCourses.Generate(1000));
            await storageBroker.SaveChangesAsync();
        }


    }

    private static Bogus.DataSets.Name.Gender ReturnGenderType(Gender gender)
    {
        switch (gender)
        {
            case Gender.Male:
                return Bogus.DataSets.Name.Gender.Male;
            case Gender.Female:
                return Bogus.DataSets.Name.Gender.Female;
            default:
                return Bogus.DataSets.Name.Gender.Male;
        }
    }

    private static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ICacheBroker, CacheBroker>();
        services.AddSingleton<IServicesLogicValidator, ServicesLogicValidator>();

        services.AddScoped<IStorageBroker, StorageBroker>();
        services.AddScoped<EndpointElapsedTimeFilter>();

        services.AddTransient(typeof(ILoggingBroker<>), typeof(LoggingBroker<>));
        services.AddTransient(typeof(IServicesExceptionsLogger<>), typeof(ServicesExceptionsLogger<>));
        services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        services.AddTransient<IUserFoundationService, UserFoundationService>();
        services.AddTransient<IUserProcessingService, UserProcessingService>();
        services.AddTransient<IUserOrchestrationService, UserOrchestrationService>();
        services.AddTransient<IAuthorFoundationService, AuthorFoundationService>();
        services.AddTransient<IAuthorProcessingService, AuthorProcessingService>();
        services.AddTransient<IAuthorOrchestrationService, AuthorOrchestrationService>();
        services.AddTransient<ICourseFoundationService, CourseFoundationService>();
        services.AddTransient<ICourseProcessingService, CourseProcessingService>();
        services.AddTransient<ICourseOrchestrationService, CourseOrchestrationService>();
        services.AddTransient<ICategoryFoundationService, CategoryFoundationService>();
        services.AddTransient<ICategoryProcessingService, CategoryProcessingService>();
        services.AddTransient<ICategoryOrchestrationService, CategoryOrchestrationService>();

        return services;
    }

    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        services.AddDbContext<StorageBroker>();

        return services;
    }

    public static IServiceCollection RegisterApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection RegisterFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>()
            .AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });

        return services;
    }

}