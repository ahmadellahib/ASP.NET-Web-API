using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Brokers.Storages;

public partial interface IStorageBroker
{
    Task<Author> InsertAuthorAsync(Author author, CancellationToken cancellationToken);
    Task<Author> UpdateAuthorAsync(Author author, CancellationToken cancellationToken);
    Task<bool> DeleteAuthorAsync(Author author, CancellationToken cancellationToken);
    IQueryable<Author> SelectAllAuthors();
    ValueTask<Author?> SelectAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);
}
