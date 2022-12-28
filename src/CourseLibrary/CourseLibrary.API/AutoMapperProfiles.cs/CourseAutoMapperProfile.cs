using AutoMapper;
using CourseLibrary.API.Contracts.Courses;
using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

internal sealed class CourseAutoMapperProfile : Profile
{
    public CourseAutoMapperProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<CourseForCreation, Course>();
        CreateMap<CourseForUpdate, Course>();
    }
}