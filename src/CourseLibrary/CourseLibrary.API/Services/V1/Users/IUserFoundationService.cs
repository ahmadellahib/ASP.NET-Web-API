using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Services.V1.Users;

public interface IUserFoundationService
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);

    Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken);

    Task RemoveUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    IQueryable<User> RetrieveAllUsers();

    IQueryable<User> SearchUsers(UserResourceParameters userResourceParameters);
}
