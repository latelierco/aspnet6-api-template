using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.Users.Add;

public class AddUserRequest
{
    [Required, MaxLength(User.FirstNameMaxLength)]
    public string Firstname { get; set; } = null!;

    [Required, MaxLength(User.LastNameMaxLength)]
    public string Lastname { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;
}
