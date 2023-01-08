using CategoryLibrary.API.Services.V1.Categories;
using CourseLibrary.API;
using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Contracts.Categories;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CategoryLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class CategoriesController : BaseController
{
    private readonly ILoggingBroker<CategoriesController> _loggingBroker;
    private readonly ICategoryOrchestrationService _categoryOrchestrationService;

    public CategoriesController(ILoggingBroker<CategoriesController> loggingBroker, ICategoryOrchestrationService categoryOrchestrationService)
    {
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _categoryOrchestrationService = categoryOrchestrationService ?? throw new ArgumentNullException(nameof(categoryOrchestrationService));
    }

    [HttpGet("{categoryId}", Name = nameof(GetCategory))]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public IActionResult GetCategory([FromRoute] Guid categoryId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
    {
        try
        {
            Category category = _categoryOrchestrationService.RetrieveCategoryById(categoryId);

            return Ok((CategoryDto)category);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCategoryAsync([FromBody] CategoryForCreation categoryForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Category addedCategory = await _categoryOrchestrationService.CreateCategoryAsync((Category)categoryForCreation, cancellationToken);

            return CreatedAtRoute(nameof(GetCategory), new { categoryId = addedCategory.Id }, (CategoryDto)addedCategory);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCategoryAsync([FromBody] CategoryForUpdate categoryForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Category storageCategory = await _categoryOrchestrationService.ModifyCategoryAsync((Category)categoryForUpdate, cancellationToken);

            return Ok((CategoryDto)storageCategory);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpGet(Name = nameof(GetAllCategoriesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            IEnumerable<Category> storageCategories = _categoryOrchestrationService.RetrieveAllCategories();

            List<CategoryDto> categoriesDtos = new();

            foreach (Category category in storageCategories)
            {
                categoriesDtos.Add((CategoryDto)category);
            }

            return Ok(categoriesDtos);
        }
        catch (Exception exception)
        {
            return (ActionResult)HandleException(exception);
        }
    }

    private IActionResult HandleException(Exception exception, IOptions<ApiBehaviorOptions>? apiBehaviorOptions = null, ActionContext? actionContext = null)
    {
        switch (exception)
        {
            case CancellationException:
                return NoContent();
            case ValidationException when exception.InnerException is NotFoundEntityException<Category>:
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
            case IDependencyException when (exception.InnerException is LockedEntityException<Category>):
                return Problem(GetInnerMessage(exception));
            case IServiceException:
                return Problem(StaticData.ControllerMessages.InternalServerError);
            default:
                _loggingBroker.LogError(exception);
                return Problem(StaticData.ControllerMessages.InternalServerError);
        }
    }
}