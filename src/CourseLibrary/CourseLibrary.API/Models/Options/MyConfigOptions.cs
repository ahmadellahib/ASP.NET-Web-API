namespace CourseLibrary.API.Models.Options;

public class MyConfigOptions
{
    public const string SectionName = "MyConfig";

    public bool UseSqlServer { get; set; } = true;
}
