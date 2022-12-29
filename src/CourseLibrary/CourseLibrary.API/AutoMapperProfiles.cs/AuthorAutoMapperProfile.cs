using AutoMapper;
using CourseLibrary.API.Contracts.Authors;
using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

internal sealed class AuthorAutoMapperProfile : Profile
{
    public AuthorAutoMapperProfile()
    {
        CreateMap<Author, AuthorDto>()
            .ForMember(author => author.MainCategory, opt => opt.MapFrom(authorDto => authorDto.MainCategory.Name));
        CreateMap<AuthorForCreation, Author>();
        CreateMap<AuthorForUpdate, Author>();
    }
}
