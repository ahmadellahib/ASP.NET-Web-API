﻿using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly ICourseOrchestrationService _courseOrchestrationService;

    public CoursesController(IMapper mapper, ICourseOrchestrationService courseOrchestrationService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _courseOrchestrationService = courseOrchestrationService ?? throw new ArgumentNullException(nameof(courseOrchestrationService));
    }

    [HttpGet("{courseId}", Name = nameof(GetCourseAsync))]
    [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetCourseAsync([FromRoute] Guid courseId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course course = await _courseOrchestrationService.RetrieveCourseByIdAsync(courseId, cancellationToken);

            return Ok(_mapper.Map<CourseDto>(course));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CourseFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCourseAsync([FromBody] CourseForCreation courseForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course course = _mapper.Map<Course>(courseForCreation);
            Course addedCourse = await _courseOrchestrationService.CreateCourseAsync(course, cancellationToken);

            return CreatedAtRoute(nameof(GetCourseAsync), new { courseId = addedCourse.Id }, _mapper.Map<CourseDto>(addedCourse));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CourseFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCourseAsync([FromBody] CourseForUpdate courseForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Course course = _mapper.Map<Course>(courseForUpdate);
            Course storageCourse = await _courseOrchestrationService.ModifyCourseAsync(course, cancellationToken);

            return Ok(_mapper.Map<CourseDto>(storageCourse));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CourseFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<Course>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CourseFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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

            return Ok(_mapper.Map<IEnumerable<CourseDto>>(storagePagedCourses));
        }
        catch (ResourceParametersException resourceParametersException)
        {
            return BadRequest(resourceParametersException.Message);
        }
        catch (DependencyException<CourseFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CourseFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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
}