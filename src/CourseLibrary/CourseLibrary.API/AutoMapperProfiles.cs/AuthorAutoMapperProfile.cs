using AutoMapper;
using CourseLibrary.API.Contracts.Authors;
using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

public class AuthorAutoMapperProfile : Profile
{
    public AuthorAutoMapperProfile()
    {
        CreateMap<Author, AuthorDto>();
        CreateMap<AuthorForCreation, Author>();
        CreateMap<AuthorForUpdate, Author>();
    }
}
