using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NinjaNye.SearchExtensions;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Infra.Persistence.SqlServer.Extensions;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users;

public class UserRepository : IReadUserRepository, IWriteUserRepository
{
    private readonly Context _context;

    public UserRepository(Context context) => _context = context;

    public async Task<(int count, IEnumerable<User> items)> GetAsync(string? search, string[] roles, bool? isActivated, string? orderBy, int pageNumber = 0, int pageSize = 50)
    {
        var users = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            users = users.Where(u => ((string) (object) u.Email).Contains(search) || u.FirstName.StartsWith(search) || u.LastName.StartsWith(search));

        if (roles.Any())
        {
            var rolePredicates = roles.Select(role => (Expression<Func<User, bool>>) (u => (',' + u.Roles + ',').Contains(',' + role + ',')));
            users = users.WhereAny(rolePredicates.ToArray());
        }

        if (isActivated.HasValue)
            users = users.Where(u => u.IsActivated == isActivated.Value);

        return (await users.CountAsync(), users.OrderBy(orderBy).Skip(pageNumber * pageSize).Take(pageSize).AsEnumerable());
    }

    public IEnumerable<User> GetUsers(IEnumerable<Guid> userId) => _context.Users.Where(u => userId.Any(uid => uid == u.Id)).AsEnumerable();

    public IEnumerable<User> GetUsersFromRoles(string[] neededRoles) => _context.Users.Search(x => x.Roles).Containing(neededRoles);

    public Task<User?> GetAsync(Guid id) => _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    public Task<User?> GetAsync(Email email) => _context.Users.SingleOrDefaultAsync(x => x.Email == email);

    public Task<bool> EmailExistsAsync(Email email) => _context.Users.AnyAsync(x => x.Email == email);

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public void Update(User user) => _context.Users.Update(user);
}
