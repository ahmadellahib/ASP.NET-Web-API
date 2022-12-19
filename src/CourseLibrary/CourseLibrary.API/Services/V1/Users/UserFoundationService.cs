using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Services.V1.Users;

public class UserFoundationService : IUserFoundationService
{
    public Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<User> RetrieveAllUsers()
    {
        throw new NotImplementedException();
    }

    public ValueTask<User?> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
