using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Services.V1.Authors;

public interface IAuthorFoundationService
{
    Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken);

    Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken);

    Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);

    IQueryable<Author> RetrieveAllAuthors();

    ValueTask<Author?> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);
}
