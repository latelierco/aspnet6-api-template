using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Template.AspNet6.Infra.Auth.Extensions;

public class TokenBuilder
{
    private string? _audience;
    private SigningCredentials? _credentials;
    private List<Claim> _claims = new();
    private DateTime _expires;
    private string? _issuer;
    private SymmetricSecurityKey? _key;

    public TokenBuilder AddClaims(List<Claim> claims)
    {
        if (_claims.Any())
            _claims.AddRange(claims);
        else
            _claims = claims;

        return this;
    }

    public TokenBuilder AddIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public TokenBuilder AddAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    public TokenBuilder AddExpiry(int minutes)
    {
        _expires = DateTime.UtcNow.AddMinutes(minutes);
        return this;
    }

    public TokenBuilder AddKey(string key)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        _credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        return this;
    }

    public JwtSecurityToken Build()
    {
        if (string.IsNullOrWhiteSpace(_issuer))
            throw new ArgumentNullException(nameof(_issuer));

        if (string.IsNullOrWhiteSpace(_audience))
            throw new ArgumentNullException(nameof(_audience));

        if (!_claims.Any())
            throw new ArgumentNullException(nameof(_claims));

        if (_expires == default)
            throw new ArgumentNullException(nameof(_expires));

        if (_credentials is null)
            throw new ArgumentNullException(nameof(_credentials));

        return new JwtSecurityToken(
            _issuer,
            _audience,
            _claims,
            expires: _expires,
            signingCredentials: _credentials
        );
    }
}