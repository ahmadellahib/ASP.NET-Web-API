using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;

namespace CourseLibrary.API.Services.V1.Users;

public interface IUserOrchestrationService
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);

    Task<User> ModifyUserAsync(User user, CancellationToken cancellationToken);

    ValueTask<User> RetrieveUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    IEnumerable<User> RetrieveAllUsers();

    PagedList<User> SearchUsers(UserResourceParameters userResourceParameters);
}