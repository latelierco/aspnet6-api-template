using Template.AspNet6.Domain.Entities.Users.Plans;
using Template.AspNet6.Domain.Entities.Users.Roles;
using Template.AspNet6.Domain.ValueObjects.Email;

#pragma warning disable CS8618

namespace Template.AspNet6.Domain.Entities.Users;

public class User : IPersistableEntity, ITimestampEntity
{
    public const int FirstNameMaxLength = 255;
    public const int LastNameMaxLength = 255;
    public const int EmailMaxLength = 255;
    public const int ProfilePictureMaxLength = 2048;
    public const int RolesMaxLength = 512;
    public const int PlansMaxLength = 512;

    public User() { }

    public User(string firstName, string lastName, Email email, string[] roles, bool isActivated, bool emailVerified, string? profilePicture)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ProfilePicture = profilePicture;
        IsEmailVerified = emailVerified;

        Roles = string.Join(CRoles.Separator, roles);
        IsActivated = isActivated;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Email Email { get; set; }
    public string? ProfilePicture { get; set; }

    public string Roles { get; set; }
    public string Plans { get; set; }

    public bool IsActivated { get; set; }
    public bool IsEmailVerified { get; set; }

    public DateTime? LastConnectionAt { get; set; }

    public IEnumerable<string> RolesAsEnumerable => Roles.Split(CRoles.Separator);
    public IEnumerable<string> PlansAsEnumerable => Plans.Split(CPlan.Separator);

    public int InternalId { get; set; }
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool HasRole(string role) => RolesAsEnumerable.Contains(role);

    public void AddRoleIfNotExists(string role) => Roles = RolesAsEnumerable.Contains(role) ? Roles : $"{Roles}{CRoles.Separator}{role}";
    public void ReplaceRoles(IEnumerable<string> roles) => Roles = string.Join(CRoles.Separator, roles);

    public void Patch(string? firstname, string? lastname)
    {
        FirstName = firstname ?? FirstName;
        LastName = lastname ?? LastName;
    }
}
