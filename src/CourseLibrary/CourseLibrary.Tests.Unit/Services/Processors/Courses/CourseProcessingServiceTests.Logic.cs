using CourseLibrary.API.Models.Courses;
using FluentAssertions;
using Force.DeepCloner;
using NSubstitute;
using Xunit;

namespace CourseLibrary.Tests.Unit.Services.Processors.Courses;

public partial class CourseProcessingServiceTests
{
    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course storageCourse = inputCourse.DeepClone();

        _courseFoundationService.CreateCourseAsync(inputCourse, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.CreateCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
    }

    [Fact]
    public async Task ModifyCourseAsync_ShouldUpdateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course storageCourse = inputCourse.DeepClone();

        _courseFoundationService.ModifyCourseAsync(inputCourse, cts)
            .Returns(storageCourse);
        // Act
        Course actualCourse = await _sut.ModifyCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
    }

    [Fact]
    public async Task RemoveCourseByIdAsync_ShouldDeleteCourse_WhenCourseExists()
    {
        // Arrange
        Guid inputCourseId = Guid.NewGuid();

        // Act
        await _sut.RemoveCourseByIdAsync(inputCourseId, cts);

        // Assert
        await _courseFoundationService.Received(1).RemoveCourseByIdAsync(inputCourseId, cts);
    }

    [Fact]
    public async Task RemoveCoursesByAuthorIdAsync_ShouldDeleteCoursesByAuthorId_WhenCoursesExists()
    {
        // Arrange
        Guid inputAuthorId = Guid.NewGuid();

        // Act
        await _sut.RemoveCoursesByAuthorIdAsync(inputAuthorId, cts);

        // Assert
        await _courseFoundationService.Received(1).RemoveCoursesByAuthorIdAsync(inputAuthorId, cts);
    }

    [Fact]
    public async Task RetrieveCourseByIdAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course storageCourse = CreateRandomCourse(dateTimeOffset);
        Guid inputCourseId = storageCourse.Id;

        _courseFoundationService.RetrieveCourseByIdAsync(inputCourseId, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.RetrieveCourseByIdAsync(inputCourseId, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnCourses_WhenSomeCoursesExists()
    {
        // Arrange
        IQueryable<Course> storageCourses = CreateRandomCourses();

        _courseFoundationService.RetrieveAllCourses()
            .Returns(storageCourses);

        // Act
        IQueryable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(storageCourses);
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnEmptyList_WhenNoCoursesExists()
    {
        // Arrange
        _courseFoundationService.RetrieveAllCourses()
            .Returns(Enumerable.Empty<Course>().AsQueryable());

        // Act
        IQueryable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(Enumerable.Empty<Course>().AsQueryable());
    }

    [Fact]
    public void SearchCourses_ShouldReturnCourses_WhenSomeCoursesExists()
    {
        // Arrange
        CourseResourceParameters courseResourceParameters = new() { OrderBy = "" };
        IQueryable<Course> storageCourses = CreateRandomCourses();

        _propertyMappingService.ValidMappingExistsFor<Course, Course>(courseResourceParameters.OrderBy)
            .Returns(true);

        _courseFoundationService.RetrieveAllCourses()
            .Returns(storageCourses);

        // Act
        IQueryable<Course> actualCourses = _sut.SearchCourses(courseResourceParameters);

        // Assert
        actualCourses.Should().BeEquivalentTo(storageCourses);
    }

    [Fact]
    public void SearchCourses_ShouldReturnEmptyList_WhenNoCoursesExists()
    {
        // Arrange
        CourseResourceParameters courseResourceParameters = new() { OrderBy = "" };

        _propertyMappingService.ValidMappingExistsFor<Course, Course>(courseResourceParameters.OrderBy)
            .Returns(true);

        _courseFoundationService.RetrieveAllCourses()
            .Returns(Enumerable.Empty<Course>().AsQueryable());

        // Act
        IQueryable<Course> actualCourses = _sut.SearchCourses(courseResourceParameters);

        // Assert
        actualCourses.Should().BeEquivalentTo(Enumerable.Empty<Course>().AsQueryable());
    }
}
