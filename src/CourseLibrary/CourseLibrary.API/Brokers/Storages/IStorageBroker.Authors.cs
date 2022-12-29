using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Brokers.Storages;

internal partial interface IStorageBroker
{
    Task<Author> InsertAuthorAsync(Author author, CancellationToken cancellationToken);
    Task<Author> UpdateAuthorAsync(Author author, CancellationToken cancellationToken);
    Task<bool> DeleteAuthorAsync(Author author, CancellationToken cancellationToken);
    ValueTask<Author?> SelectAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);
    IQueryable<Author> SelectAllAuthors();
}