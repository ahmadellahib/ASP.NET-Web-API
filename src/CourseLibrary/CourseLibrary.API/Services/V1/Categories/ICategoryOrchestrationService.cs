﻿using CourseLibrary.API.Models.Categories;

namespace CategoryLibrary.API.Services.V1.Categories;

public interface ICategoryOrchestrationService
{
    Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken);

    Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken);

    ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);

    IEnumerable<Category> RetrieveAllCategories();
}