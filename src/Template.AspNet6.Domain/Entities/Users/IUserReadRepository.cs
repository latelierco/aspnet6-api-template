using Template.AspNet6.Domain.ValueObjects.Email;

namespace Template.AspNet6.Domain.Entities.Users;

public interface IReadUserRepository
{
    IEnumerable<User> GetUsers(IEnumerable<Guid> userId);
    Task<(int count, IEnumerable<User> items)> GetAsync(string? search, string[] roles, bool? isActivated, string? orderBy, int pageNumber = 0, int pageSize = 50);
    IEnumerable<User> GetUsersFromRoles(string[] neededRoles);
    Task<User?> GetAsync(Email email);
    Task<User?> GetAsync(Guid id);
    Task<bool> EmailExistsAsync(Email email);
}
