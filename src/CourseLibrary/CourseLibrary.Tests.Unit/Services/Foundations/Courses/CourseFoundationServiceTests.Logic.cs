using CourseLibrary.API;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Validators.Courses;
using FluentAssertions;
using Force.DeepCloner;
using NSubstitute;
using Xunit;

namespace CourseLibrary.Tests.Unit.Services.Foundations.Courses;

public partial class CourseFoundationServiceTests
{
    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();

        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course storageCourse = inputCourse.DeepClone();

        _storageBroker.InsertCourseAsync(inputCourse, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.CreateCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
        _servicesLogicValidator.Received(1).ValidateEntity(inputCourse, Arg.Any<CourseValidator>());
        await _storageBroker.Received(1).InsertCourseAsync(inputCourse, cts);

        Received.InOrder(() =>
        {
            _servicesLogicValidator.ValidateEntity(inputCourse, Arg.Any<CourseValidator>());
            _storageBroker.InsertCourseAsync(inputCourse, cts);
        });
    }

    [Fact]
    public async Task ModifyCourseAsync_ShouldUpdateCourse_WhenDetailsValid()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course inputCourse = CreateRandomCourse(dateTimeOffset);
        Course storageCourse = inputCourse.DeepClone();

        _storageBroker.UpdateCourseAsync(inputCourse, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.ModifyCourseAsync(inputCourse, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
        _servicesLogicValidator.Received(1).ValidateEntity(inputCourse, Arg.Any<CourseValidator>());
        await _storageBroker.Received(1).UpdateCourseAsync(inputCourse, cts);

        Received.InOrder(() =>
        {
            _servicesLogicValidator.ValidateEntity(inputCourse, Arg.Any<CourseValidator>());
            _storageBroker.UpdateCourseAsync(inputCourse, cts);
        });
    }

    [Fact]
    public async Task RemoveCourseByIdAsync_ShouldDeleteCourse_WhenCourseExists()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course storageCourse = CreateRandomCourse(dateTimeOffset);
        Guid inputCourseId = storageCourse.Id;

        _storageBroker.SelectCourseByIdAsync(inputCourseId, cts)
            .Returns(storageCourse);

        _storageBroker.DeleteCourseAsync(storageCourse, cts)
            .Returns(true);

        // Act
        await _sut.RemoveCourseByIdAsync(inputCourseId, cts);

        // Assert
        _servicesLogicValidator.Received(1).ValidateParameter(inputCourseId, "courseId");
        await _storageBroker.Received(1).SelectCourseByIdAsync(inputCourseId, cts);
        _servicesLogicValidator.Received(1).ValidateStorageEntity<Course>(storageCourse, inputCourseId);
        await _storageBroker.Received(1).DeleteCourseAsync(storageCourse, cts);

        Received.InOrder(() =>
        {
            _servicesLogicValidator.ValidateParameter(inputCourseId, "courseId");
            _storageBroker.SelectCourseByIdAsync(inputCourseId, cts);
            _servicesLogicValidator.ValidateStorageEntity<Course>(storageCourse, inputCourseId);
            _storageBroker.DeleteCourseAsync(storageCourse, cts);
        });
    }

    [Fact]
    public async Task RemoveCoursesByAuthorIdAsync_ShouldDeleteCourseOfAuthor_WhenSomeAuthorsCoursesExists()
    {
        // Arrange
        Guid inputAuthorId = Guid.NewGuid();

        // Act
        await _sut.RemoveCoursesByAuthorIdAsync(inputAuthorId, cts);

        // Assert
        await _storageBroker.Received(1).DeleteCoursesByAuthorIdAsync(inputAuthorId, cts);
    }

    [Fact]
    public async Task RetrieveCourseByIdAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = GetRandomDateTime();
        Course storageCourse = CreateRandomCourse(dateTimeOffset);
        Guid inputCourseId = storageCourse.Id;

        _storageBroker.SelectCourseByIdAsync(inputCourseId, cts)
            .Returns(storageCourse);

        // Act
        Course actualCourse = await _sut.RetrieveCourseByIdAsync(inputCourseId, cts);

        // Assert
        actualCourse.Should().BeEquivalentTo(storageCourse);
        _servicesLogicValidator.Received(1).ValidateParameter(inputCourseId, "courseId");
        await _storageBroker.Received(1).SelectCourseByIdAsync(inputCourseId, cts);
        _servicesLogicValidator.Received(1).ValidateStorageEntity<Course>(storageCourse, inputCourseId);

        Received.InOrder(() =>
        {
            _servicesLogicValidator.ValidateParameter(inputCourseId, "courseId");
            _storageBroker.SelectCourseByIdAsync(inputCourseId, cts);
            _servicesLogicValidator.ValidateStorageEntity<Course>(storageCourse, inputCourseId);
        });
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnCourses_WhenSomeCoursesExists()
    {
        // Arrange
        IQueryable<Course> storageCourses = CreateRandomCourses();

        _storageBroker.SelectAllCourses()
                .Returns(storageCourses);

        // Act
        IQueryable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(storageCourses);
        _storageBroker.Received(1).SelectAllCourses();
        _loggingBroker.DidNotReceive().LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
    }

    [Fact]
    public void RetrieveAllCourses_ShouldReturnEmptyList_WhenNoCoursesExists()
    {
        // Arrange
        _storageBroker.SelectAllCourses()
                 .Returns(Enumerable.Empty<Course>().AsQueryable());

        // Act
        IQueryable<Course> actualCourses = _sut.RetrieveAllCourses();

        // Assert
        actualCourses.Should().BeEquivalentTo(Enumerable.Empty<Course>().AsQueryable());
        _storageBroker.Received(1).SelectAllCourses();
        _loggingBroker.Received(1).LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);

        Received.InOrder(() =>
        {
            _storageBroker.SelectAllCourses();
            _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
        });
    }
}