using AutoMapper;
using CourseLibrary.API.Contracts.Users;
using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.AutoMapperProfiles.cs;

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserForCreation, User>();
        CreateMap<UserForUpdate, User>();
    }
}