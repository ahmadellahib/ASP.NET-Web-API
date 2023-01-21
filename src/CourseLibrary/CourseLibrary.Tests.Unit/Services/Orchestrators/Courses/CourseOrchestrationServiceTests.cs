using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Services;
using CourseLibrary.API.Services.V1.Courses;
using NSubstitute;

namespace CourseLibrary.Tests.Unit.Services.Orchestrators.Courses;

public partial class CourseOrchestrationServiceTests : BaseServiceTest
{
    private readonly CourseOrchestrationService _sut;
    private readonly CancellationToken cts = new CancellationTokenSource().Token;
    private readonly ICourseProcessingService _courseProcessingService = Substitute.For<ICourseProcessingService>();
    private readonly IServicesLogicValidator _servicesLogicValidator = Substitute.For<IServicesLogicValidator>();
    private readonly IServicesExceptionsLogger<CourseOrchestrationService> _servicesExceptionsLogger = Substitute.For<IServicesExceptionsLogger<CourseOrchestrationService>>();

    public CourseOrchestrationServiceTests()
    {
        _sut = new CourseOrchestrationService(_courseProcessingService,
            _servicesLogicValidator,
            _servicesExceptionsLogger);
    }

    private Course CreateRandomCourse(DateTimeOffset dateTimeOffset) =>
       FillersCreator.CreateCourseFiller(dateTimeOffset).Create();

    private IQueryable<Course> CreateRandomCourses() =>
        FillersCreator.CreateCourseFiller(GetRandomDateTime()).Create(GetRandomNumber()).AsQueryable();
}
