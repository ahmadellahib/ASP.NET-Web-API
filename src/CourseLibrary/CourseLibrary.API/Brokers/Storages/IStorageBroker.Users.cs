using CourseLibrary.API.Models.Users;

namespace CourseLibrary.API.Brokers.Storages;

internal partial interface IStorageBroker
{
    Task<User> InsertUserAsync(User user, CancellationToken cancellationToken);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
    ValueTask<User?> SelectUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    IQueryable<User> SelectAllUsers();
}
