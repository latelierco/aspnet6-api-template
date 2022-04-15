namespace Template.AspNet6.Domain.Entities.Users;

public class UserClaim
{
    public UserClaim(User user, IAuthToken accessToken, IAuthToken refreshToken)
    {
        User = user;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public User User { get; set; }

    public IAuthToken AccessToken { get; set; }
    public IAuthToken RefreshToken { get; set; }
}
