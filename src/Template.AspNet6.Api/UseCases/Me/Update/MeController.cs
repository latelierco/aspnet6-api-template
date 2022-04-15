using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Api.UseCases.Users;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Application.UseCases.Users.Update;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Me.Update;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MeController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.UpdateUserFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.NotFound() => _viewModel = NotFound(new ProblemDetails());

    void IOutputPort.Ok(User user) => _viewModel = Ok(new UserResponse(user));

    /// <summary>
    ///     Update authenticated user
    /// </summary>
    [HttpPatch]
    [Authorize(Roles = CRoles.User)]
    [SwaggerOperation(Tags = new[] {"Me"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateMeAsync(
        [FromServices] IIdentityProvider identityProvider,
        [FromServices] IUpdateUserUseCase useCase,
        [FromBody] UpdateMeRequest request)
    {
        useCase.SetOutputPort(this);

        var identity = identityProvider.GetCurrentIdentity();

        await useCase.ExecuteAsync(identity!.UserId, request.Firstname, request.Lastname);

        return _viewModel!;
    }
}
