using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.ValueObjects.Email;
using Microsoft.AspNetCore.Http;
using Template.AspNet6.Domain.Entities.Users.Plans;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Infra.Auth;

public class IdentityProvider : IIdentityProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityProvider(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Identity? GetCurrentIdentity()
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims.ToList();

        if (claims == null || !claims.Any()) return null;

        var roles = string.Join(CRoles.Separator, claims.Where(x => x.Type == ClaimData.Roles.Output).Select(x => x.Value));
        var plans = string.Join(CPlan.Separator, claims.Where(x => x.Type == ClaimData.Plans.Output).Select(x => x.Value));

        var firstName = claims.FirstOrDefault(x => x.Type == ClaimData.GivenName.Output)?.Value;
        var lastName = claims.FirstOrDefault(x => x.Type == ClaimData.FamilyName.Output)?.Value;
        var email = claims.FirstOrDefault(x => x.Type == ClaimData.Email.Output)?.Value;

        bool? activated = null;
        var isActivatedClaim = claims.FirstOrDefault(x => x.Type == ClaimData.IsActivated.Output);
        if (isActivatedClaim != null)
            activated = bool.Parse(isActivatedClaim.Value);

        bool? emailVerified = null;
        var emailVerifiedClaim = claims.FirstOrDefault(x => x.Type == ClaimData.IsEmailVerified.Output);
        if (emailVerifiedClaim != null)
            emailVerified = bool.Parse(emailVerifiedClaim.Value);

        var impersonatedIdClaim = claims.FirstOrDefault(x => x.Type == ClaimData.ImpersonatedId.Output);
        var impersonatedId = impersonatedIdClaim == null ? (Guid?) null : Guid.Parse(impersonatedIdClaim.Value);

        var userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimData.UserId.Output);
        var userId = userIdClaim == null ? throw new ArgumentNullException(nameof(userIdClaim)) : Guid.Parse(userIdClaim.Value);

        return new Identity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = string.IsNullOrEmpty(email) ? null : new Email(email),
            Roles = roles,
            Plans = plans,
            IsActivated = activated,
            IsEmailVerified = emailVerified,
            UserId = userId,
            ImpersonatedId = impersonatedId
        };
    }
}
