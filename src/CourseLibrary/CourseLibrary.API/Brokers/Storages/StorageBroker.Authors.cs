using CourseLibrary.API.Models.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CourseLibrary.API.Brokers.Storages;

internal sealed partial class StorageBroker
{
    internal DbSet<Author> Authors { get; set; }

    public async Task<Author> InsertAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        EntityEntry<Author> authorEntityEntry = await Authors.AddAsync(author, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return authorEntityEntry.Entity;
    }

    public async Task<Author> UpdateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        EntityEntry<Author> authorEntityEntry = Authors.Update(author);
        await SaveChangesAsync(cancellationToken);

        return authorEntityEntry.Entity;
    }

    public async Task<bool> DeleteAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        Authors.Remove(author);
        int result = await SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async ValueTask<Author?> SelectAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
         await Authors.Include(x => x.MainCategory)
                      .Include(x => x.User)
                      .SingleOrDefaultAsync(x => x.Id == authorId, cancellationToken);

    public IQueryable<Author> SelectAllAuthors() =>
        Authors.Include(x => x.MainCategory)
               .Include(x => x.User);
}
