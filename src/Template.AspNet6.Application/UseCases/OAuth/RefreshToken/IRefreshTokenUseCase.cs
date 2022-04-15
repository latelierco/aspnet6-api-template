namespace Template.AspNet6.Application.UseCases.OAuth.RefreshToken;

public interface IRefreshTokenUseCase
{
    Task ExecuteAsync(string accessToken, string refreshToken);

    void SetOutputPort(IOutputPort outputPort);
}