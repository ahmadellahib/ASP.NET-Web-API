using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Pagination;
using FluentAssertions;
using Force.DeepCloner;
using NSubstitute;
using Xunit;

namespace CourseLibrary.Tests.Unit.Services.Orchestrators.Courses;

public partial class CourseOrchestrationServiceTests
{
    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course createdCourse = CreateRandomCourse(dateTimeOffset);
        Course expectedCourse = createdCourse.DeepClone();

        _courseProcessingService.CreateCourseAsync(inputCourse, cts)
            .Returns(createdCourse);

        // Act
        Course actualCourse = await _sut.CreateCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(expectedCourse);
        await _courseProcessingService.Received(1).CreateCourseAsync(inputCourse, cts);
    }

    [Fact]
    public async Task ModifyCourseAsync_ShouldUpdateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course storageCourse = CreateRandomCourse(dateTimeOffset);
        Course modifiedCourse = CreateRandomCourse(dateTimeOffset);
        Course expectedCourse = modifiedCourse.DeepClone();

        _courseProcessingService.RetrieveCourseByIdAsync(inputCourse.Id, cts)
            .Returns(storageCourse);

        _courseProcessingService.ModifyCourseAsync(inputCourse, cts)
            .Returns(modifiedCourse);

        // Act
        Course actualCourse = await _sut.ModifyCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(expectedCourse);
        await _courseProcessingService.Received(1).RetrieveCourseByIdAsync(inputCourse.Id, cts);
        _servicesLogicValidator.Received(1).ValidateEntityConcurrency<Course>(inputCourse, storageCourse);
        await _courseProcessingService.Received(1).ModifyCourseAsync(inputCourse, cts);

        Received.InOrder(() =>
        {
            _courseProcessingService.RetrieveCourseByIdAsync(inputCourse.Id, cts);
            _servicesLogicValidator.ValidateEntityConcurrency<Course>(inputCourse, storageCourse);
            _courseProcessingService.ModifyCourseAsync(inputCourse, cts);
        });
    }

    [Fact]
    public async Task RemoveCourseByIdAsync_ShouldDeleteCourse_WhenCourseExists()
    {
        // Arrange
        Guid inputCourseId = Guid.NewGuid();

        // Act
        await _sut.RemoveCourseByIdAsync(inputCourseId, cts);

        // Assert
        await _courseProcessingService.Received(1).RemoveCourseByIdAsync(inputCourseId, cts);
    }

    [Fact]
    public async Task RemoveCoursesByAuthorIdAsync_ShouldDeleteAuthorCourses_WhenCoursesExists()
    {
        // Arrange
        Guid inputAuthorId = Guid.NewGuid();

        // Act
        await _sut.RemoveCoursesByAuthorIdAsync(inputAuthorId, cts);

        // Assert
        await _courseProcessingService.Received(1).RemoveCoursesByAuthorIdAsync(inputAuthorId, cts);
    }

    [Fact]
    public async Task RetrieveCourseByIdAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course storageCourse = CreateRandomCourse(dateTimeOffset);
        Course expectedCourse = storageCourse.DeepClone();
        Guid inputCourseId = storageCourse.Id;

        _courseProcessingService.RetrieveCourseByIdAsync(inputCourseId, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.RetrieveCourseByIdAsync(inputCourseId, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(expectedCourse);
        await _courseProcessingService.Received(1).RetrieveCourseByIdAsync(inputCourseId, cts);
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnCourses_WhenSomeCoursesExists()
    {
        // Arrange
        IQueryable<Course> storageCourses = CreateRandomCourses();
        IEnumerable<Course> expectedCourses = storageCourses.AsEnumerable().DeepClone();

        _courseProcessingService.RetrieveAllCourses()
            .Returns(storageCourses);

        // Act
        IEnumerable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(expectedCourses);
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnEmptyList_WhenNoCoursesExists()
    {
        // Arrange
        _courseProcessingService.RetrieveAllCourses()
            .Returns(Enumerable.Empty<Course>().AsQueryable());

        // Act
        IEnumerable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(Enumerable.Empty<Course>());
    }

    [Fact]
    public void SearchCourses_ShouldReturnCourses_WhenSomeCoursesExists()
    {
        // Arrange
        CourseResourceParameters courseResourceParameters = new();
        IQueryable<Course> storageCourses = CreateRandomCourses();
        IEnumerable<Course> expectedCourses = PagedList<Course>.Create(storageCourses.DeepClone(), courseResourceParameters.PageNumber, courseResourceParameters.PageSize);

        _courseProcessingService.SearchCourses(courseResourceParameters)
            .Returns(storageCourses);

        // Act
        IEnumerable<Course> actualCourses = _sut.SearchCourses(courseResourceParameters);

        // Assert
        actualCourses.Should().BeEquivalentTo(expectedCourses);
    }

    [Fact]
    public void SearchCourses_ShouldReturnEmptyList_WhenNoCoursesExists()
    {
        // Arrange
        CourseResourceParameters courseResourceParameters = new();

        _courseProcessingService.SearchCourses(courseResourceParameters)
            .Returns(Enumerable.Empty<Course>().AsQueryable());

        // Act
        IEnumerable<Course> actualCourses = _sut.SearchCourses(courseResourceParameters);

        // Assert
        actualCourses.Should().BeEquivalentTo(Enumerable.Empty<Course>());
    }
}
