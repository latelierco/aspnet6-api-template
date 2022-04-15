namespace Template.AspNet6.Application.UseCases.OAuth.SignIn;

public interface ISignInUseCase
{
    Task ExecuteAsync(SignInPasswordInput input);
    Task ExecuteCustomOpenIdAsync(string externalToken);
    Task ExecuteGoogleAsync(string accessToken);
    Task ExecuteMicrosoftAsync(string externalToken);

    void SetOutputPort(IOutputPort outputPort);
}