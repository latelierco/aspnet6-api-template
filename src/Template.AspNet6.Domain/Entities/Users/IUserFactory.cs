using Template.AspNet6.Domain.Entities.Users.Claims;
using Template.AspNet6.Domain.ValueObjects.Email;

namespace Template.AspNet6.Domain.Entities.Users;

public interface IUserFactory
{
    User NewUser(string firstName, string lastName, Email email);
    User NewActivatedUser(string firstName, string lastName, Email email, string? profilePicture = null);
    Claim NewClaim(User user, string provider, string token, DateTime expirationDate);
}
