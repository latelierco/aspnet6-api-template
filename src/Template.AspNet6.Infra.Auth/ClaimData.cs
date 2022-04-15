using System.Security.Claims;

namespace Template.AspNet6.Infra.Auth;

public static class ClaimData
{
    public static readonly ClaimType Email = new("email", ClaimTypes.Email);
    public static readonly ClaimType FamilyName = new("lastName", "lastName");
    public static readonly ClaimType GivenName = new("firstName", "firstName");
    public static readonly ClaimType IsActivated = new("isActivated", "isActivated");
    public static readonly ClaimType IsEmailVerified = new("isEmailVerified", "isEmailVerified");
    public static readonly ClaimType Roles = new("roles", ClaimTypes.Role);
    public static readonly ClaimType Plans = new("plans", "plans");
    public static readonly ClaimType UserId = new("userId", "userId");
    public static readonly ClaimType ImpersonatedId = new("impersonatedId", "impersonatedId");

    public class ClaimType
    {
        public ClaimType(string input, string output)
        {
            Input = input;
            Output = output;
        }

        public string Input { get; set; }
        public string Output { get; set; }
    }
}
