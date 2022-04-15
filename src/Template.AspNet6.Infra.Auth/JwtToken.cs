using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Infra.Auth;

public class JwtToken : IAuthToken
{
    public JwtToken(string token, DateTime expirationDate)
    {
        Token = token;
        ExpirationDate = expirationDate;
    }

    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
}
