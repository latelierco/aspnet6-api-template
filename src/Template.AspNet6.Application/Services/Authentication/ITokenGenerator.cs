using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.Services.Authentication;

public interface ITokenGenerator
{
    string GenerateAccessToken(Identity identity, int expirationInMinutes);

    Guid GetUserIdFromExpiredToken(string token);
    string GenerateRefreshToken();
    string GenerateSignupToken();
}
