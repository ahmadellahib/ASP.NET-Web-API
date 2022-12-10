using CourseLibrary.API.Models.Authors;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Brokers.Storages;

internal partial class StorageBroker
{
    internal DbSet<Author> Authors { get; set; }

    public Task<Author> InsertAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Author> UpdateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Author> SelectAllAuthors()
    {
        throw new NotImplementedException();
    }

    public ValueTask<Author?> SelectAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
