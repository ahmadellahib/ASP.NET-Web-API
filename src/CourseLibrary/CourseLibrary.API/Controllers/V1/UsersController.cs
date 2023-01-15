using CourseLibrary.API.Contracts.Users;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class UsersController : BaseController<UsersController>
{
    private readonly IUserOrchestrationService _userOrchestrationService;

    public UsersController(IUserOrchestrationService userOrchestrationService)
    {
        _userOrchestrationService = userOrchestrationService ?? throw new ArgumentNullException(nameof(userOrchestrationService));
    }

    [HttpGet("{userId}", Name = nameof(GetUserAsync))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetUserAsync([FromRoute] Guid userId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User user = await _userOrchestrationService.RetrieveUserByIdAsync(userId, cancellationToken);

            return Ok((UserDto)user);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostUserAsync([FromBody] UserForCreation userForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User addedUser = await _userOrchestrationService.CreateUserAsync((User)userForCreation, cancellationToken);

            return CreatedAtRoute(nameof(GetUserAsync), new { userId = addedUser.Id }, (UserDto)addedUser);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchUserAsync([FromRoute] Guid userId, [FromBody] UserForUpdate userForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User user = (User)userForUpdate;
            user.Id = userId;
            User storageUser = await _userOrchestrationService.ModifyUserAsync(user, cancellationToken);

            return Ok((UserDto)storageUser);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpGet(Name = nameof(SearchUsersAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<UserDto>> SearchUsersAsync([FromQuery] UserResourceParameters userResourceParameters)
    {
        try
        {
            PagedList<User> storagePagedUsers = _userOrchestrationService.SearchUsers(userResourceParameters);

            PaginationMetaData PaginationMetaData = new()
            {
                TotalCount = storagePagedUsers.TotalCount,
                PageSize = storagePagedUsers.PageSize,
                CurrentPage = storagePagedUsers.CurrentPage,
                TotalPages = storagePagedUsers.TotalPages,
                HasPrevious = storagePagedUsers.HasPrevious,
                HasNext = storagePagedUsers.HasNext,
                PreviousPageLink = storagePagedUsers.HasPrevious ? CreateUserResourceUri(userResourceParameters, ResourceUriType.PreviousPage) : string.Empty,
                NextPageLink = storagePagedUsers.HasNext ? CreateUserResourceUri(userResourceParameters, ResourceUriType.NextPage) : string.Empty
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(PaginationMetaData));

            List<UserDto> usersDtos = new();

            foreach (User user in storagePagedUsers)
            {
                usersDtos.Add((UserDto)user);
            }

            return Ok(usersDtos);
        }
        catch (Exception exception)
        {
            return (ActionResult)HandleException(exception);
        }
    }

    private string CreateUserResourceUri(UserResourceParameters userResourceParameters, ResourceUriType type)
    {
        string? resourceUri;

        switch (type)
        {
            case ResourceUriType.PreviousPage:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber - 1,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
                    });
                break;

            case ResourceUriType.NextPage:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber + 1,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
                    });
                break;

            default:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
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
            case ValidationException when exception.InnerException is NotFoundEntityException<User>:
                return NotFound(GetInnerMessage(exception));
            case ValidationException:
                if (apiBehaviorOptions is null || actionContext is null)
                    throw new ArgumentNullException(nameof(apiBehaviorOptions));

                SetModelState(ModelState, (ValidationException)exception);

                return apiBehaviorOptions!.Value.InvalidModelStateResponseFactory(actionContext!);
            case IDependencyException when (exception.InnerException is DbConflictException):
                return Conflict(exception.Message);
            case IDependencyException when (exception.InnerException is LockedEntityException<User>):
                return Problem(GetInnerMessage(exception));
            case IServiceException:
                return Problem(StaticData.ControllerMessages.InternalServerError);
            default:
                LoggingBroker.LogError(exception);
                return Problem(StaticData.ControllerMessages.InternalServerError);
        }
    }
}