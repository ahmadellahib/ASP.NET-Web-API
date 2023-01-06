using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.Brokers.Caches;

public partial interface ICacheBroker
{
    List<Category> GetCachedCategories();
    void SetCachedCategories(List<Category> categoriesList);
    void ClearCachedCategories();
}