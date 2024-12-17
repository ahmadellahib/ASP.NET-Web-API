using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Services;
using CourseLibrary.API.Services.V1.Courses;
using NSubstitute;

namespace CourseLibrary.Tests.Unit.Services.Foundations.Courses;

public partial class CourseFoundationServiceTests : BaseServiceTest
{
    private readonly CourseFoundationService _sut;
    private readonly CancellationToken cts = new CancellationTokenSource().Token;
    private readonly IStorageBroker _storageBroker = Substitute.For<IStorageBroker>();
    private readonly ILoggingBroker<CourseFoundationService> _loggingBroker = Substitute.For<ILoggingBroker<CourseFoundationService>>();
    private readonly IServicesLogicValidator _servicesLogicValidator = Substitute.For<IServicesLogicValidator>();
    
    public CourseFoundationServiceTests()
    {
        _sut = new CourseFoundationService(_storageBroker, _servicesLogicValidator, _loggingBroker);
    }

    private Course CreateRandomCourse(DateTimeOffset dateTimeOffset) =>
       FillersCreator.CreateCourseFiller(dateTimeOffset).Create();

    private IQueryable<Course> CreateRandomCourses() =>
        FillersCreator.CreateCourseFiller(GetRandomDateTime()).Create(GetRandomNumber()).AsQueryable();
}
