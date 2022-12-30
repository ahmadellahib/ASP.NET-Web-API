using AutoMapper;
using CourseLibrary.API.Contracts.Courses;
using CourseLibrary.API.Models.Courses;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

internal sealed class CourseAutoMapperProfile : Profile
{
    public CourseAutoMapperProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(courseDto => courseDto.AuthorFirstName, opt => opt.MapFrom(course => course.Author.User.FirstName))
            .ForMember(courseDto => courseDto.AuthorLastName, opt => opt.MapFrom(course => course.Author.User.LastName));

        CreateMap<CourseForCreation, Course>();

        CreateMap<CourseForUpdate, Course>();
    }
}