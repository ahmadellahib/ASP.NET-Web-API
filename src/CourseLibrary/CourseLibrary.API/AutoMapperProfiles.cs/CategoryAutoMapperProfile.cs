using AutoMapper;
using CourseLibrary.API.Contracts.Categories;
using CourseLibrary.API.Models.Categories;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

internal sealed class CategoryAutoMapperProfile : Profile
{
    public CategoryAutoMapperProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryForCreation, Category>();
        CreateMap<CategoryForUpdate, Category>();
    }
}