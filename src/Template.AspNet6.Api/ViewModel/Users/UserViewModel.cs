using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.ViewModel.Users;

public class UserViewModel
{
    public UserViewModel(User user)
    {
        Id = user.Id;

        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email.Value;

        IsActivated = user.IsActivated;
        EmailVerified = user.IsEmailVerified;
        Roles = user.RolesAsEnumerable;
        Plans = user.PlansAsEnumerable;

        LastConnectionAt = user.LastConnectionAt;
    }

    public Guid Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string? Email { get; set; }
    public bool IsActivated { get; set; }
    public bool EmailVerified { get; set; }

    public IEnumerable<string> Roles { get; set; }
    public IEnumerable<string> Plans { get; set; }

    public DateTime? LastConnectionAt { get; set; }
}
