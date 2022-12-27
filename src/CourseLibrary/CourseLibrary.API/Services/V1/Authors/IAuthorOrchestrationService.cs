using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Authors;

public interface IAuthorOrchestrationService
{
    Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken);

    Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken);

    Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);

    ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken);

    IEnumerable<Author> RetrieveAllAuthors();

    PagedList<Author> SearchAuthors(AuthorResourceParameters authorResourceParameters);
}
