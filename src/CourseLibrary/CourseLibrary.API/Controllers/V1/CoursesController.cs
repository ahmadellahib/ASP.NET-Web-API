using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Contracts.Courses;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Courses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class CoursesController : BaseController
{
    private readonly ILoggingBroker<CoursesController> _loggingBroker;
    private readonly ICourseOrchestrationService _courseOrchestrationService;

    public CoursesController(ILoggingBroker<CoursesController> loggingBroker, ICourseOrchestrationService courseOrchestrationService)
    {
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _courseOrchestrationService = courseOrchestrationService ?? throw new ArgumentNullException(nameof(courseOrchestrationService));
    }

    [HttpGet("{courseId}", Name = nameof(GetCourseAsync))]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetCourseAsync([FromRoute] Guid courseId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course course = await _courseOrchestrationService.RetrieveCourseByIdAsync(courseId, cancellationToken);

            return Ok((CourseDto)course);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CourseCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCourseAsync([FromBody] CourseForCreation courseForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course addedCourse = await _courseOrchestrationService.CreateCourseAsync((Course)courseForCreation, cancellationToken);

            return CreatedAtRoute(nameof(GetCourseAsync), new { courseId = addedCourse.Id }, (CourseCreatedDto)addedCourse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(CourseUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCourseAsync([FromBody] CourseForUpdate courseForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course storageCourse = await _courseOrchestrationService.ModifyCourseAsync((Course)courseForUpdate, cancellationToken);

            return Ok((CourseUpdatedDto)storageCourse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpDelete("{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCourseAsync(Guid courseId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            await _courseOrchestrationService.RemoveCourseByIdAsync(courseId, cancellationToken);

            return NoContent();
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpGet(Name = nameof(SearchCoursesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<CourseDto>> SearchCoursesAsync([FromQuery] CourseResourceParameters courseResourceParameters)
    {
        try
        {
            PagedList<Course> storagePagedCourses = _courseOrchestrationService.SearchCourses(courseResourceParameters);

            PaginationMetaData PaginationMetaData = new()
            {
                TotalCount = storagePagedCourses.TotalCount,
                PageSize = storagePagedCourses.PageSize,
                CurrentPage = storagePagedCourses.CurrentPage,
                TotalPages = storagePagedCourses.TotalPages,
                HasPrevious = storagePagedCourses.HasPrevious,
                HasNext = storagePagedCourses.HasNext,
                PreviousPageLink = storagePagedCourses.HasPrevious ? CreateCourseResourceUri(courseResourceParameters, ResourceUriType.PreviousPage) : string.Empty,
                NextPageLink = storagePagedCourses.HasNext ? CreateCourseResourceUri(courseResourceParameters, ResourceUriType.NextPage) : string.Empty
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(PaginationMetaData));

            List<CourseDto> coursesDtos = new();

            foreach (Course course in storagePagedCourses)
            {
                coursesDtos.Add((CourseDto)course);
            }

            return Ok(coursesDtos);
        }
        catch (Exception exception)
        {
            return (ActionResult)HandleException(exception);
        }
    }

    private string CreateCourseResourceUri(CourseResourceParameters courseResourceParameters, ResourceUriType type)
    {
        string? resourceUri;

        switch (type)
        {
            case ResourceUriType.PreviousPage:
                resourceUri = Url.Link(nameof(SearchCoursesAsync),
                    new
                    {
                        orderBy = courseResourceParameters.OrderBy,
                        pageNumber = courseResourceParameters.PageNumber - 1,
                        pageSize = courseResourceParameters.PageSize,
                        searchQuery = courseResourceParameters.SearchQuery
                    });
                break;

            case ResourceUriType.NextPage:
                resourceUri = Url.Link(nameof(SearchCoursesAsync),
                    new
                    {
                        orderBy = courseResourceParameters.OrderBy,
                        pageNumber = courseResourceParameters.PageNumber + 1,
                        pageSize = courseResourceParameters.PageSize,
                        searchQuery = courseResourceParameters.SearchQuery
                    });
                break;

            default:
                resourceUri = Url.Link(nameof(SearchCoursesAsync),
                    new
                    {
                        orderBy = courseResourceParameters.OrderBy,
                        pageNumber = courseResourceParameters.PageNumber,
                        pageSize = courseResourceParameters.PageSize,
                        searchQuery = courseResourceParameters.SearchQuery
                    });
                break;
        }

        return resourceUri is null ? string.Empty : resourceUri.Replace("http://", "https://");
    }

    private IActionResult HandleException(Exception exception, IOptions<ApiBehaviorOptions>? apiBehaviorOptions = null, ActionContext? actionContext = null)
    {
        switch (exception)
        {
            case ResourceParametersException:
                return BadRequest(exception.Message);
            case CancellationException:
                return NoContent();
            case ValidationException when exception.InnerException is NotFoundEntityException<Course>:
                return NotFound(GetInnerMessage(exception));
            case ValidationException:
                if (apiBehaviorOptions is null || actionContext is null)
                {
                    throw new ArgumentNullException(nameof(apiBehaviorOptions));
                }

                SetModelState(ModelState, (ValidationException)exception);

                return apiBehaviorOptions!.Value.InvalidModelStateResponseFactory(actionContext!);
            case IDependencyException when (exception.InnerException is DbConflictException):
                return Conflict(exception.Message);
            case IDependencyException when (exception.InnerException is LockedEntityException<Course>):
                return Problem(GetInnerMessage(exception));
            case IServiceException:
                return Problem(StaticData.ControllerMessages.InternalServerError);
            default:
                _loggingBroker.LogError(exception);
                return Problem(StaticData.ControllerMessages.InternalServerError);
        }
    }
}
