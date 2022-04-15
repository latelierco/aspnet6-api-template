using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.UseCases.OAuth.SignIn;
using Template.AspNet6.Domain.Common;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.OAuth.SignIn;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.InvalidInput(ModelState modelState) => _viewModel = BadRequest(new ValidationProblemDetails(modelState.ErrorMessages));
    void IOutputPort.SignInFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.UserNotFound(string detail, string title, int code) => _viewModel = NotFound(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.Ok(UserClaim claim) => _viewModel = Ok(new SignInResponse(claim));

    /// <summary>
    ///     Sign in user
    /// </summary>
    [HttpPost("signin/password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [SwaggerOperation(Tags = new[] {"Auth"})]
    public async Task<IActionResult> SignInWithPasswordAsync([FromServices] ISignInUseCase useCase, [FromBody] SignInWithPasswordRequest request)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(request.ToInput());

        return _viewModel!;
    }

    /// <summary>
    ///     Sign in user with custom sso token
    /// </summary>
    [HttpPost("signin/custom/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [SwaggerOperation(Tags = new[] {"Auth.Providers"})]
    public async Task<IActionResult> SignInWithCustomOpenIdAsync([FromServices] ISignInUseCase useCase, [FromRoute] string token)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteCustomOpenIdAsync(token);

        return _viewModel!;
    }

    /// <summary>
    ///     Sign in user with google access token
    /// </summary>
    [HttpPost("signin/google/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [SwaggerOperation(Tags = new[] {"Auth.Providers"})]
    public async Task<IActionResult> SignInWithGoogleAsync([FromServices] ISignInUseCase useCase, [FromRoute] string token)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteGoogleAsync(token);

        return _viewModel!;
    }

    /// <summary>
    ///     Sign in user with microsoft sso token
    /// </summary>
    [HttpPost("signin/microsoft/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [SwaggerOperation(Tags = new[] {"Auth.Providers"})]
    public async Task<IActionResult> SignInWithMicrosoftAsync([FromServices] ISignInUseCase useCase, [FromRoute] string token)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteMicrosoftAsync(token);

        return _viewModel!;
    }
}
