using AutoMapper;
using CourseLibrary.API.Contracts.Authors;
using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

internal sealed class AuthorAutoMapperProfile : Profile
{
    public AuthorAutoMapperProfile()
    {
        CreateMap<Author, AuthorDto>()
            .ForMember(authorDto => authorDto.FirstName, opt => opt.MapFrom(author => author.User.FirstName))
            .ForMember(authorDto => authorDto.LastName, opt => opt.MapFrom(author => author.User.LastName))
            .ForMember(authorDto => authorDto.MainCategory, opt => opt.MapFrom(author => author.MainCategory.Name));

        CreateMap<AuthorForCreation, Author>();

        CreateMap<AuthorForUpdate, Author>();
    }
}
