using CourseLibrary.API.Models.Authors;

namespace CourseLibrary.API.Services.V1.Authors;

public class AuthorFoundationService : IAuthorFoundationService
{
    public Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Author> RetrieveAllAuthors()
    {
        throw new NotImplementedException();
    }

    public ValueTask<Author?> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
