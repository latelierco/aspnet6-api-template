using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Claims;
using Template.AspNet6.Domain.Entities.Users.Roles;
using Template.AspNet6.Domain.ValueObjects.Email;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users;

public class UserFactory : IUserFactory
{
    public User NewUser(string firstName, string lastName, Email email) => new(firstName, lastName, email, new[] {CRoles.User}, false, false, null);
    public User NewActivatedUser(string firstName, string lastName, Email email, string? profilePicture = null) => new(firstName, lastName, email, new[] {CRoles.User}, true, true, !string.IsNullOrWhiteSpace(profilePicture) ? profilePicture : null);

    public Claim NewClaim(User user, string provider, string token, DateTime expirationDate) => new(user.Id, provider, token, expirationDate);
}
