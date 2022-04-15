using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Application.Services.Persistence;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.OAuth.SignIn;

public class SignInUseCase : ISignInUseCase
{
    private readonly IAuthenticator _auth;
    private readonly IReadUserRepository _readUsers;
    private readonly IUnitOfWork _unitOfWork;
    private IOutputPort? _outputPort;

    public SignInUseCase(IAuthenticator auth, IReadUserRepository readUsers, IUnitOfWork unitOfWork)
    {
        _auth = auth;
        _readUsers = readUsers;
        _unitOfWork = unitOfWork;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteCustomOpenIdAsync(string externalToken)
    {
        try
        {
            var claim = await _auth.SignInWithCustomOpenIdAsync(externalToken);
            await _unitOfWork.SaveAsync();

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.SignInFailed(e.Message);
        }
    }

    public async Task ExecuteAsync(SignInPasswordInput input)
    {
        if (input.ModelState.IsInvalid)
        {
            _outputPort?.InvalidInput(input.ModelState);
            return;
        }

        try
        {
            var user = await _readUsers.GetAsync(input.Email);
            if (user is null)
            {
                _outputPort?.UserNotFound();
                return;
            }

            var claim = await _auth.SignInWithPasswordAsync(user, input.Password);
            await _unitOfWork.SaveAsync();

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.SignInFailed(e.Message);
        }
    }

    public async Task ExecuteGoogleAsync(string accessToken)
    {
        try
        {
            var claim = await _auth.SignInWithGoogleAsync(accessToken);
            await _unitOfWork.SaveAsync();

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.SignInFailed(e.Message);
        }
    }

    public async Task ExecuteMicrosoftAsync(string externalToken)
    {
        try
        {
            var claim = await _auth.SignInWithMicrosoftAsync(externalToken);
            await _unitOfWork.SaveAsync();

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.SignInFailed(e.Message);
        }
    }
}
