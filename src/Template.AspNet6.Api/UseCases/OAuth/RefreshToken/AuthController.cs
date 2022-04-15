using Microsoft.AspNetCore.Mvc;
using Template.AspNet6.Application.UseCases.OAuth.RefreshToken;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.OAuth.RefreshToken;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.RefreshTokenFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.Ok(UserClaim claim) => _viewModel = Ok(new AuthResponse(claim));

    /// <summary>
    /// Refresh access token  
    /// </summary>
    /// <remarks>Access Token can be expired.</remarks>
    [HttpPost("refreshtoken", Name = "refresh user access token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> RefreshAccessTokenAsync([FromServices] IRefreshTokenUseCase useCase, [FromBody] RefreshTokenRequest options)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(options.AccessToken, options.RefreshToken);

        return _viewModel!;
    }
}
