using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Services.V1.Courses;
using CourseLibrary.API.Services.V1.PropertyMappings;
using NSubstitute;

namespace CourseLibrary.Tests.Unit.Services.Processors.Courses;

public partial class CourseProcessingServiceTests : BaseServiceTest
{
    private readonly CourseProcessingService _sut;
    private readonly CancellationToken cts = new CancellationTokenSource().Token;
    private readonly ICourseFoundationService _courseFoundationService = Substitute.For<ICourseFoundationService>();
    private readonly IPropertyMappingService _propertyMappingService = Substitute.For<IPropertyMappingService>();
    
    public CourseProcessingServiceTests()
    {
        _sut = new CourseProcessingService(_courseFoundationService, _propertyMappingService);
    }

    private Course CreateRandomCourse(DateTimeOffset dateTimeOffset) =>
        FillersCreator.CreateCourseFiller(dateTimeOffset).Create();

    private IQueryable<Course> CreateRandomCourses() =>
        FillersCreator.CreateCourseFiller(GetRandomDateTime()).Create(GetRandomNumber()).AsQueryable();
}
