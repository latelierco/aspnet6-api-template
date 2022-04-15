using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Claims;
using Template.AspNet6.Domain.ValueObjects.Password;

namespace Template.AspNet6.Application.Services.Authentication;

public interface IAuthenticator
{
    /// <summary>
    ///     SignUp a new user based on password and email
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns>verificationToken for email verification and claims to handle access / permission</returns>
    Task<(string verificationToken, UserClaim userClaim)> SignUpAsync(User user, Password password);

    /// <summary>
    ///     Confirm account with email verification token sent during signup
    /// </summary>
    /// <param name="verificationToken"></param>
    /// <returns></returns>
    Task<UserClaim> ConfirmSignUpAsync(string verificationToken);

    /// <summary>
    ///     SignIn a user based on email and password
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<UserClaim> SignInWithPasswordAsync(User user, Password password);

    /// <summary>
    ///     SignIn a user based on custom sso implementation
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<UserClaim> SignInWithCustomOpenIdAsync(string code);

    /// <summary>
    ///     SignIn a user based on google access token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    Task<UserClaim> SignInWithGoogleAsync(string accessToken);

    /// <summary>
    ///     SignIn a user based on microsoft sso implementation
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<UserClaim> SignInWithMicrosoftAsync(string code);

    /// <summary>
    ///     Give fresh access to user based on his refresh token
    /// </summary>
    /// <param name="accessToken">expired accessToken</param>
    /// <param name="refreshToken">valid refreshToken</param>
    /// <returns></returns>
    Task<UserClaim> RefreshAccessTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    ///     Revoke password user access
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task RevokePasswordClaimIfAnyAsync(User user);

    /// <summary>
    ///     Revoke refresh token user access
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task RevokeRefreshTokenIfAnyAsync(User user);

    /// <summary>
    ///     Ask for a new password with email verification process
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<Claim> ClaimNewPasswordAsync(string email);

    /// <summary>
    ///     Confirm new password with email verification token sent during password claim
    /// </summary>
    /// <param name="password"></param>
    /// <param name="verificationToken"></param>
    /// <returns></returns>
    Task<UserClaim> ConfirmNewPasswordAsync(string password, string verificationToken);

    /// <summary>
    ///     Often use for administrator, it give same access than targeted user
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="targetUser"></param>
    /// <returns></returns>
    UserClaim LogAs(Identity currentUser, User targetUser);
}
