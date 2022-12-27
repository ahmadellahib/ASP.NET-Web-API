using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Brokers.Storages;

public partial interface IStorageBroker
{
    Task<User> InsertUserAsync(User user, CancellationToken cancellationToken);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
    IQueryable<User> SelectAllUsers();
    ValueTask<User?> SelectUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
