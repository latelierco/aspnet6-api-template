using Template.AspNet6.Domain.Common;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Domain.ValueObjects.Password;

namespace Template.AspNet6.Application.UseCases.OAuth.SignIn;

public class SignInPasswordInput
{
    public SignInPasswordInput(string email, string password)
    {
        ModelState = new ModelState();

        try
        {
            if (!string.IsNullOrWhiteSpace(email))
                Email = new Email(email);
        }
        catch (EmailException e)
        {
            ModelState.Add(nameof(email), e.Message);
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(password))
                Password = new Password(password);
        }
        catch (PasswordException e)
        {
            ModelState.Add(nameof(password), e.Message);
        }
    }

    internal Email Email { get; set; } = null!;
    internal Password Password { get; set; } = null!;
    internal ModelState ModelState { get; }
}
