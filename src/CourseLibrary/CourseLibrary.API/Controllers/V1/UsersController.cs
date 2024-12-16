using CourseLibrary.API.Contracts.Users;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Asp.Versioning;

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
        User user = await _userOrchestrationService.RetrieveUserByIdAsync(userId, cancellationToken);

        return Ok((UserDto)user);
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
        User addedUser = await _userOrchestrationService.CreateUserAsync((User)userForCreation, cancellationToken);

        return CreatedAtRoute(nameof(GetUserAsync), new { userId = addedUser.Id }, (UserDto)addedUser);
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
        User user = (User)userForUpdate;
        user.Id = userId;
        User storageUser = await _userOrchestrationService.ModifyUserAsync(user, cancellationToken);

        return Ok((UserDto)storageUser);
    }

    [HttpGet(Name = nameof(SearchUsersAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<UserDto>> SearchUsersAsync([FromQuery] UserResourceParameters userResourceParameters)
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

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(PaginationMetaData));

        List<UserDto> usersDto = storagePagedUsers.Select(user => (UserDto)user).ToList();

        return Ok(usersDto);
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
}