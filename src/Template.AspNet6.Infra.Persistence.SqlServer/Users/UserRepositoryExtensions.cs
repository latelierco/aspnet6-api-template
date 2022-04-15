using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users;

public static class UserRepositoryExtensions
{
    public static IQueryable<User> OrderBy(this IQueryable<User> users, string? orderBy = null)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            orderBy = string.Empty;

        var isDesc = orderBy.StartsWith("-");

        if (orderBy.Equals("name", StringComparison.InvariantCultureIgnoreCase))
            users = isDesc ? users.OrderByDescending(u => u.LastName) : users.OrderBy(u => u.LastName);
        else if (orderBy.Equals("email", StringComparison.InvariantCultureIgnoreCase))
            users = isDesc ? users.OrderByDescending(u => u.Email.Value) : users.OrderBy(u => u.Email.Value);
        else
            users = users.OrderByDescending(u => u.LastConnectionAt);

        return users;
    }
}
