using Asp.Versioning;
using CourseLibrary.API.Contracts.Categories;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Categories;
using CourseLibrary.API.Services.V1.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class CategoriesController : BaseController<CategoriesController>
{
    private readonly ICategoryOrchestrationService _categoryOrchestrationService;

    public CategoriesController(ICategoryOrchestrationService categoryOrchestrationService)
    {
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
        Category category = _categoryOrchestrationService.RetrieveCategoryById(categoryId);

        return Ok((CategoryDto)category);
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
        Category addedCategory = await _categoryOrchestrationService.CreateCategoryAsync((Category)categoryForCreation, cancellationToken);

        return CreatedAtRoute(nameof(GetCategory), new { categoryId = addedCategory.Id }, (CategoryDto)addedCategory);
    }

    [HttpPatch("{categoryId}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchCategoryAsync([FromRoute] Guid categoryId, [FromBody] CategoryForUpdate categoryForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        Category category = (Category)categoryForUpdate;
        category.Id = categoryId;
        Category storageCategory = await _categoryOrchestrationService.ModifyCategoryAsync(category, cancellationToken);

        return Ok((CategoryDto)storageCategory);
    }

    [HttpGet(Name = nameof(GetAllCategoriesAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        List<CategoryDto> categoriesDto = _categoryOrchestrationService.RetrieveAllCategories()
            .Select(category => (CategoryDto)category)
            .ToList();

        return Ok(categoriesDto);
    }
}