using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Domain.Entities;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Claims;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Domain.ValueObjects.Password;
using Template.AspNet6.Infra.Auth.Providers;

namespace Template.AspNet6.Infra.Auth;

public class Authenticator : IAuthenticator
{
    private readonly int _accessTokenExpirationInMinutes;

    private readonly IClaimReadRepository _readClaims;
    private readonly IReadUserRepository _readUsers;
    private readonly int _refreshTokenExpirationInMonths;

    private readonly ProviderFactory _ssoProviderFactory;

    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserFactory _userFactory;
    private readonly int _verificationTokenExpirationInDays;
    private readonly IClaimWriteRepository _writeClaims;
    private readonly IWriteUserRepository _writeUsers;


    public Authenticator(IConfiguration configuration, ITokenGenerator tokenGenerator, IUserFactory userFactory, IWriteUserRepository writeUsers,
        IReadUserRepository readUsers, IClaimReadRepository readClaims, IClaimWriteRepository writeClaims, TelemetryClient telemetry)
    {
        _tokenGenerator = tokenGenerator;
        _userFactory = userFactory;
        _writeUsers = writeUsers;
        _readUsers = readUsers;
        _readClaims = readClaims;
        _writeClaims = writeClaims;

        if (int.TryParse(configuration["OAuth:BuiltIn:AccessTokenExpirationInMinutes"], out var accessTokenExpirationInMinutes))
            _accessTokenExpirationInMinutes = accessTokenExpirationInMinutes;
        else throw new ArgumentNullException(nameof(accessTokenExpirationInMinutes));

        if (int.TryParse(configuration["OAuth:BuiltIn:RefreshTokenExpirationInMonths"], out var refreshTokenExpirationInMonths))
            _refreshTokenExpirationInMonths = refreshTokenExpirationInMonths;
        else throw new ArgumentNullException(nameof(refreshTokenExpirationInMonths));

        if (int.TryParse(configuration["OAuth:BuiltIn:RefreshTokenExpirationInMonths"], out var verificationTokenExpirationInDays))
            _verificationTokenExpirationInDays = verificationTokenExpirationInDays;
        else throw new ArgumentNullException(nameof(verificationTokenExpirationInDays));

        _ssoProviderFactory = new ProviderFactory(configuration, userFactory, telemetry);
    }

    #region PRIVATE

    private async Task<UserClaim> ChangePasswordAsync(User user, Password newPassword)
    {
        await RevokePasswordClaimIfAnyAsync(user);
        var passwordToAdd = _userFactory.NewClaim(user, Constants.PasswordProviderName, newPassword.HashedPassword, DateTime.MaxValue);

        var (newAccessToken, newRefreshToken) = GenerateAccessAndRefreshTokenPair(user, _accessTokenExpirationInMinutes);

        await RevokeRefreshTokenIfAnyAsync(user);
        var claimRefreshToken = _userFactory.NewClaim(user, Constants.RefreshTokenProviderName, newRefreshToken, DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths));

        await _writeClaims.AddClaimAsync(passwordToAdd);
        await _writeClaims.AddClaimAsync(claimRefreshToken);

        return new UserClaim(user,
            new JwtToken(newAccessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(claimRefreshToken.Value, claimRefreshToken.ExpirationDate));
    }

    private (string accessToken, string refreshToken) GenerateAccessAndRefreshTokenPair(User user, int expirationInMinutes, Guid? impersonatedId = null)
    {
        var identity = GetIdentityFromUserId(user, impersonatedId);
        var newAccessToken = _tokenGenerator.GenerateAccessToken(identity, expirationInMinutes);
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

        return (newAccessToken, newRefreshToken);
    }

    private Identity GetIdentityFromUserId(User user, Guid? impersonatedId = null)
    {
        var identity = new Identity
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user!.Email,
            Roles = user.Roles,
            Plans = user.Plans,
            IsActivated = user.IsActivated,
            IsEmailVerified = user.IsEmailVerified,
            ImpersonatedId = impersonatedId
        };

        return identity;
    }

    private async Task EnsureUserPasswordMatch(IIdentifiable user, Password password)
    {
        var currentPasswordHashClaim = await _readClaims.GetClaimValueAsync(user.Id, Constants.PasswordProviderName);
        Password.EnsureHashMatch(password.ClearPassword, currentPasswordHashClaim);
    }

    private async Task<UserClaim> GenerateClaimAccessAsync(User user)
    {
        var (newAccessToken, newRefreshToken) = GenerateAccessAndRefreshTokenPair(user, _accessTokenExpirationInMinutes);

        var newClaimRefreshToken = _userFactory.NewClaim(user, Constants.RefreshTokenProviderName, newRefreshToken, DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths));
        await _writeClaims.AddClaimAsync(newClaimRefreshToken);

        return new UserClaim(user,
            new JwtToken(newAccessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(newClaimRefreshToken.Value, newClaimRefreshToken.ExpirationDate));
    }

    #endregion

    #region SIGN_IN

    public async Task<UserClaim> SignInWithPasswordAsync(User user, Password password)
    {
        await EnsureUserPasswordMatch(user, password);

        var currentRefreshToken = await _readClaims.GetClaimAsync(user.Id, Constants.RefreshTokenProviderName);
        if (currentRefreshToken is null)
            throw new ArgumentNullException(nameof(currentRefreshToken));

        var (newAccessToken, newRefreshToken) = GenerateAccessAndRefreshTokenPair(user, _accessTokenExpirationInMinutes);

        currentRefreshToken.Value = newRefreshToken;
        //We are updating refresh expiration date only for activated user.
        if (currentRefreshToken.User.IsEmailVerified)
            currentRefreshToken.ExpirationDate = DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths);

        _writeClaims.UpdateClaim(currentRefreshToken);

        return new UserClaim(currentRefreshToken.User,
            new JwtToken(newAccessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(currentRefreshToken.Value, currentRefreshToken.ExpirationDate));
    }

    public async Task<UserClaim> SignInWithCustomOpenIdAsync(string code)
    {
        var provider = _ssoProviderFactory.Create(ProviderType.Custom);
        var user = await provider.GetUserAsync(code);

        user.LastConnectionAt = DateTime.UtcNow;

        return await GenerateClaimAccessAsync(user);
    }

    public async Task<UserClaim> SignInWithGoogleAsync(string accessToken)
    {
        var provider = _ssoProviderFactory.Create(ProviderType.Google);
        var user = await provider.GetUserAsync(accessToken);

        user.LastConnectionAt = DateTime.UtcNow;

        return await GenerateClaimAccessAsync(user);
    }

    public async Task<UserClaim> SignInWithMicrosoftAsync(string code)
    {
        var provider = _ssoProviderFactory.Create(ProviderType.Microsoft);
        var user = await provider.GetUserAsync(code);

        user.LastConnectionAt = DateTime.UtcNow;

        return await GenerateClaimAccessAsync(user);
    }

    public UserClaim LogAs(Identity currentUser, User targetUser)
    {
        var tokens = GenerateAccessAndRefreshTokenPair(targetUser, _accessTokenExpirationInMinutes, currentUser.UserId);

        return new UserClaim(targetUser,
            new JwtToken(tokens.accessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(tokens.refreshToken, DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths)));
    }

    #endregion

    #region SIGN_UP

    public async Task<(string verificationToken, UserClaim userClaim)> SignUpAsync(User user, Password password)
    {
        if (await _readUsers.EmailExistsAsync(user.Email)) throw new Exception("user already exists");

        await _writeUsers.AddAsync(user);

        var (accessToken, refreshToken) = GenerateAccessAndRefreshTokenPair(user, _accessTokenExpirationInMinutes);

        var newPassword = _userFactory.NewClaim(user, Constants.PasswordProviderName, password.HashedPassword, DateTime.MaxValue);
        //Here we are assigning the same expiration datetime as verification token so the user has 72h until email confirmed.
        var claimRefreshToken = _userFactory.NewClaim(user, Constants.RefreshTokenProviderName, refreshToken, DateTime.UtcNow.AddDays(_verificationTokenExpirationInDays));
        var claimVerificationToken = _userFactory.NewClaim(user, Constants.EmailVerificationTokenProviderName, _tokenGenerator.GenerateRefreshToken(), DateTime.UtcNow.AddDays(_verificationTokenExpirationInDays));

        await _writeClaims.AddClaimAsync(newPassword);
        await _writeClaims.AddClaimAsync(claimRefreshToken);
        await _writeClaims.AddClaimAsync(claimVerificationToken);

        return (claimVerificationToken.Value, new UserClaim(user,
            new JwtToken(accessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(refreshToken, claimRefreshToken.ExpirationDate)));
    }

    public async Task<UserClaim> ConfirmSignUpAsync(string verificationToken)
    {
        var verificationTokenClaim = await _readClaims.GetClaimAsync(verificationToken, Constants.EmailVerificationTokenProviderName);

        if (verificationTokenClaim is null) throw new Exception("verification not valid");

        await RevokeRefreshTokenIfAnyAsync(verificationTokenClaim.User);

        var (accessToken, refreshToken) = GenerateAccessAndRefreshTokenPair(verificationTokenClaim.User, _accessTokenExpirationInMinutes);

        var claimRefreshToken = _userFactory.NewClaim(verificationTokenClaim.User, Constants.RefreshTokenProviderName, refreshToken, DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths));

        verificationTokenClaim.User.IsEmailVerified = true;
        verificationTokenClaim.User.IsActivated = true;

        _writeUsers.Update(verificationTokenClaim.User);
        await _writeClaims.AddClaimAsync(claimRefreshToken);
        _writeClaims.RemoveClaim(verificationTokenClaim);

        return new UserClaim(verificationTokenClaim.User,
            new JwtToken(accessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(refreshToken, claimRefreshToken.ExpirationDate));
    }

    #endregion

    #region PASSWORD_UPDATE

    public async Task RevokePasswordClaimIfAnyAsync(User user)
    {
        var oldPasswordClaim = await _readClaims.GetClaimAsync(user.Id, Constants.PasswordProviderName);
        if (oldPasswordClaim is not null)
            _writeClaims.RemoveClaim(oldPasswordClaim);
    }

    public async Task<Claim> ClaimNewPasswordAsync(string email)
    {
        var user = await _readUsers.GetAsync(new Email(email));
        if (user is null) throw new Exception("user not found");

        var expiration = DateTime.UtcNow.AddDays(_verificationTokenExpirationInDays);
        var verificationToken = _userFactory.NewClaim(user, Constants.ResetPasswordTokenProviderName, _tokenGenerator.GenerateRefreshToken(), expiration);

        await _writeClaims.AddClaimAsync(verificationToken);

        return verificationToken;
    }

    public async Task<UserClaim> ConfirmNewPasswordAsync(string password, string verificationToken)
    {
        var verificationTokenClaim = await _readClaims.GetClaimAsync(verificationToken, Constants.ResetPasswordTokenProviderName);
        if (verificationTokenClaim is null)
            throw new Exception("verification not valid");

        var user = verificationTokenClaim.User;

        _writeClaims.RemoveClaim(verificationTokenClaim);

        return await ChangePasswordAsync(user, new Password(password));
    }

    #endregion

    #region TOKEN

    public async Task<UserClaim> RefreshAccessTokenAsync(string accessToken, string refreshToken)
    {
        var currentRefreshToken = await _readClaims.GetClaimAsync(refreshToken, Constants.RefreshTokenProviderName);

        if (currentRefreshToken == null || currentRefreshToken.Value != refreshToken || currentRefreshToken.IsExpired)
            throw new Exception("current refresh token has expired");

        var (newAccessToken, newRefreshToken) = GenerateAccessAndRefreshTokenPair(currentRefreshToken.User, _accessTokenExpirationInMinutes);

        currentRefreshToken.Value = newRefreshToken;
        currentRefreshToken.ExpirationDate = DateTime.UtcNow.AddMonths(_refreshTokenExpirationInMonths);

        _writeClaims.UpdateClaim(currentRefreshToken);

        return new UserClaim(currentRefreshToken.User,
            new JwtToken(newAccessToken, DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes)),
            new JwtToken(currentRefreshToken.Value, currentRefreshToken.ExpirationDate));
    }

    public async Task RevokeRefreshTokenIfAnyAsync(User user)
    {
        var refreshToken = await _readClaims.GetClaimAsync(user.Id, Constants.RefreshTokenProviderName);
        if (refreshToken is not null)
            _writeClaims.RemoveClaim(refreshToken);
    }

    #endregion
}