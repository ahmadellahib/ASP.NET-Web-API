using System.Diagnostics.Metrics;

namespace CourseLibrary.API.Diagnostics;

public static class DiagnosticsConfig
{
    public const string ServiceName = "CourseLibrary";
    public const string ServiceVersion = "1.0.0";

    public static Meter Meter = new(ServiceName);

    // custom metrics
    public static Counter<int> CoursesVisited = Meter.CreateCounter<int>("courses.visited.count");
}
