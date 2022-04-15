using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Application.UseCases.OAuth.SignIn;

namespace Template.AspNet6.Api.UseCases.OAuth.SignIn;

public class SignInWithPasswordRequest
{
    [Required] [MinLength(5)] [MaxLength(50)] [EmailAddress]
    public string Email { get; set; } = null!;

    [Required] [MinLength(5)] [MaxLength(50)]
    public string Password { get; set; } = null!;

    public SignInPasswordInput ToInput() => new(Email, Password);
}
