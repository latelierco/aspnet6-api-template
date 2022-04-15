using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Application.Services.Persistence;

namespace Template.AspNet6.Application.UseCases.OAuth.RefreshToken;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly IAuthenticator _auth;
    private readonly IUnitOfWork _unitOfWork;
    private IOutputPort? _outputPort;

    public RefreshTokenUseCase(IAuthenticator auth, IUnitOfWork unitOfWork)
    {
        _auth = auth;
        _unitOfWork = unitOfWork;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(string accessToken, string refreshToken)
    {
        try
        {
            var claim = await _auth.RefreshAccessTokenAsync(accessToken, refreshToken);
            await _unitOfWork.SaveAsync();

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.RefreshTokenFailed(e.Message);
        }
    }
}
