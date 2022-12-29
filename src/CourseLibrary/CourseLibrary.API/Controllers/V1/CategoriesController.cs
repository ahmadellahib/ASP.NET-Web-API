using AutoMapper;
using CategoryLibrary.API.Services.V1.Categories;
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
    private readonly IMapper _mapper;
    private readonly ICategoryOrchestrationService _categoryOrchestrationService;

    public CategoriesController(IMapper mapper, ICategoryOrchestrationService categoryOrchestrationService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _categoryOrchestrationService = categoryOrchestrationService ?? throw new ArgumentNullException(nameof(categoryOrchestrationService));
    }

    [HttpGet("{categoryId}", Name = nameof(GetCategoryAsync))]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetCategoryAsync([FromRoute] Guid categoryId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Category category = await _categoryOrchestrationService.RetrieveCategoryByIdAsync(categoryId, cancellationToken);

            return Ok(_mapper.Map<CategoryDto>(category));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Category>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CategoryFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCategoryAsync([FromBody] CategoryForCreation categoryForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Category category = _mapper.Map<Category>(categoryForCreation);
            Category addedCategory = await _categoryOrchestrationService.CreateCategoryAsync(category, cancellationToken);

            return CreatedAtRoute(nameof(GetCategoryAsync), new { categoryId = addedCategory.Id }, _mapper.Map<CategoryDto>(addedCategory));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Category>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CategoryFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCategoryAsync([FromBody] CategoryForUpdate categoryForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Category category = _mapper.Map<Category>(categoryForUpdate);
            Category storageCategory = await _categoryOrchestrationService.ModifyCategoryAsync(category, cancellationToken);

            return Ok(_mapper.Map<CategoryDto>(storageCategory));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Category>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<Category>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CategoryFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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

            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(storageCategories));
        }
        catch (DependencyException<CategoryFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<CategoryFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }
}
