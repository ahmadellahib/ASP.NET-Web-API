using Bogus;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Enums;
using CourseLibrary.API.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.IoC;

internal static class StartupHelperExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<StorageBroker>(ServiceLifetime.Scoped);

        return builder.Build();
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
            ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private static async Task SeedData(StorageBroker storageBroker)
    {
        if (!storageBroker.Users.Any())
        {
            //Set the randomizer seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);

            string[] category = new[] { "Science", "Cultural Studies", "Art Studio", "Wellness and Health", "Creative Writing", "Business", "Technology and Data Science" };

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

            Faker<Author> testAuthors = new Faker<Author>()
                .StrictMode(false)
                .RuleFor(x => x.UserId, f => f.PickRandom(users).Id)
                .RuleFor(x => x.MainCategory, f => f.PickRandom(category))
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
}