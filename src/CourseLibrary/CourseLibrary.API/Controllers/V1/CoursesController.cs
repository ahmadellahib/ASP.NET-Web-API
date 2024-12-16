using CourseLibrary.API.Contracts.Courses;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Courses;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Courses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Asp.Versioning;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class CoursesController : BaseController<CoursesController>
{
    private readonly ICourseOrchestrationService _courseOrchestrationService;

    public CoursesController(ICourseOrchestrationService courseOrchestrationService)
    {
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
        Course course = await _courseOrchestrationService.RetrieveCourseByIdAsync(courseId, cancellationToken);

        return Ok((CourseDto)course);
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
        Course addedCourse = await _courseOrchestrationService.CreateCourseAsync((Course)courseForCreation, cancellationToken);

        return CreatedAtRoute(nameof(GetCourseAsync), new { courseId = addedCourse.Id }, (CourseCreatedDto)addedCourse);
    }

    [HttpPatch("{courseId}")]
    [ProducesResponseType(typeof(CourseUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchCourseAsync([FromRoute] Guid courseId, [FromBody] CourseForUpdate courseForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        Course course = (Course)courseForUpdate;
        course.Id = courseId;
        Course storageCourse = await _courseOrchestrationService.ModifyCourseAsync(course, cancellationToken);

        return Ok((CourseUpdatedDto)storageCourse);
    }

    [HttpDelete("{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCourseAsync(Guid courseId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        await _courseOrchestrationService.RemoveCourseByIdAsync(courseId, cancellationToken);

        return NoContent();
    }

    [HttpGet(Name = nameof(SearchCoursesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<CourseDto>> SearchCoursesAsync([FromQuery] CourseResourceParameters courseResourceParameters)
    {
        PagedList<Course> storagePagedCourses = _courseOrchestrationService.SearchCourses(courseResourceParameters);

        PaginationMetaData paginationMetaData = new()
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

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

        List<CourseDto> coursesDto = storagePagedCourses.Select(course => (CourseDto)course).ToList();

        return Ok(coursesDto);
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
}
