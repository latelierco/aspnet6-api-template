using System.Security.Claims;
using Template.AspNet6.Domain.Entities.Users.Plans;
using Template.AspNet6.Domain.ValueObjects.Email;

namespace Template.AspNet6.Domain.Entities.Users;

public class Identity : ClaimsPrincipal
{
    public Guid UserId { get; set; }
    public Guid? ImpersonatedId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Email? Email { get; set; }
    public bool? IsActivated { get; set; }
    public bool? IsEmailVerified { get; set; }
    public string? Roles { get; set; }
    public string? Plans { get; set; }

    public bool HasRole(string role) => Roles?.Split(",")?.Contains(role) ?? false;
    public bool HasPlan(string plan) => Plans?.Split(CPlan.Separator)?.Contains(plan, StringComparer.InvariantCultureIgnoreCase) ?? false;


    public bool HasNotRole(string role) => !HasRole(role);
}
