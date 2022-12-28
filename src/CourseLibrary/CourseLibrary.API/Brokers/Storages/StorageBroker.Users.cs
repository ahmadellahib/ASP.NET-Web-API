using CourseLibrary.API.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CourseLibrary.API.Brokers.Storages;

internal sealed partial class StorageBroker
{
    internal DbSet<User> Users { get; set; }

    public async Task<User> InsertUserAsync(User user, CancellationToken cancellationToken)
    {
        EntityEntry<User> userEntityEntry = await Users.AddAsync(user, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return userEntityEntry.Entity;
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        EntityEntry<User> userEntityEntry = Users.Update(user);
        await SaveChangesAsync(cancellationToken);

        return userEntityEntry.Entity;
    }

    public IQueryable<User> SelectAllUsers() =>
        Users.AsQueryable();

    public async ValueTask<User?> SelectUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
         await Users.FindAsync(new object[] { userId }, cancellationToken);
}