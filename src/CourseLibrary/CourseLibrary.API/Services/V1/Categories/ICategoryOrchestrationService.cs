﻿using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Services.V1.Categories;

public interface ICategoryOrchestrationService
{
    Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken);

    Task<Category> ModifyCategoryAsync(Category category, CancellationToken cancellationToken);

    Category RetrieveCategoryById(Guid categoryId);

    IEnumerable<Category> RetrieveAllCategories();
}