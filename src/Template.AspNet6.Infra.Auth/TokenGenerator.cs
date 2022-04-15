using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Plans;
using Template.AspNet6.Domain.Entities.Users.Roles;
using Template.AspNet6.Infra.Auth.Extensions;

namespace Template.AspNet6.Infra.Auth;

public class TokenGenerator : ITokenGenerator
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _secretKey;

    public TokenGenerator(IConfiguration configuration)
    {
        _audience = configuration["OAuth:BuiltIn:Audience"];
        if (string.IsNullOrWhiteSpace(_audience)) throw new ArgumentNullException(nameof(_audience));
        _issuer = configuration["OAuth:BuiltIn:Issuer"];
        if (string.IsNullOrWhiteSpace(_issuer)) throw new ArgumentNullException(nameof(_issuer));
        _secretKey = configuration["OAuth:BuiltIn:ClientSecret"];
        if (string.IsNullOrWhiteSpace(_secretKey)) throw new ArgumentNullException(nameof(_secretKey));
    }

    public string GenerateAccessToken(Identity identity, int expirationInMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(identity.Roles)) throw new ArgumentNullException(nameof(identity.Roles));

        var claims = new List<Claim>();

        claims.AddRange(identity.Roles.Split(CRoles.Separator).Select(r => new Claim(ClaimData.Roles.Input, r)));
        claims.AddRange(identity.Plans?.Split(CPlan.Separator).Select(r => new Claim(ClaimData.Plans.Input, r)) ?? new List<Claim>());

        claims.Add(new Claim(ClaimData.UserId.Input, $"{identity.UserId}"));

        if (identity.ImpersonatedId.HasValue)
            claims.Add(new Claim(ClaimData.ImpersonatedId.Input, $"{identity.ImpersonatedId.Value}"));
        if (identity.LastName != null)
            claims.Add(new Claim(ClaimData.FamilyName.Input, identity.LastName));
        if (identity.FirstName != null)
            claims.Add(new Claim(ClaimData.GivenName.Input, identity.FirstName));
        if (identity.Email?.Value != null)
            claims.Add(new Claim(ClaimData.Email.Input, identity.Email.Value));
        if (identity.IsActivated.HasValue)
            claims.Add(new Claim(ClaimData.IsActivated.Input, $"{identity.IsActivated.Value}"));
        if (identity.IsEmailVerified.HasValue)
            claims.Add(new Claim(ClaimData.IsEmailVerified.Input, $"{identity.IsEmailVerified.Value}"));

        var token = new TokenBuilder()
            .AddAudience(_audience)
            .AddIssuer(_issuer)
            .AddExpiry(expirationInMinutes)
            .AddKey(_secretKey)
            .AddClaims(claims)
            .Build();

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid GetUserIdFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return new Guid(principal.Claims.First(x => x.Type == ClaimData.UserId.Output).Value);
    }

    public string GenerateRefreshToken()
        => GenerateUrlSafeRandomToken(32);

    public string GenerateSignupToken()
        => GenerateUrlSafeRandomToken(32);

    private static string GenerateUrlSafeRandomToken(int size)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber).Replace("+", "-").Replace("/", "_");
    }
}