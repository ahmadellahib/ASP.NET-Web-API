using IDP.API.Models;

namespace IDP.API.Services.Authentication;
public interface IAuthenticationService
{
    string Login(UserCredentials credentials);
}