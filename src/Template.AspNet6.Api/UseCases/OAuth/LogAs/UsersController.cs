using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.UseCases.OAuth.LogAs;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.OAuth.LogAs;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class UsersController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.Forbidden() => _viewModel = Forbid();
    void IOutputPort.LogAsFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});

    void IOutputPort.Ok(UserClaim claim) => _viewModel = Ok(new AdminLogAsResponse(claim));

    /// <summary>
    ///     Get temporary user access token
    /// </summary>
    [HttpPost("{id:guid}/accesstoken", Name = "get temporary user access token")]
    [Authorize(Roles = CRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminLogAsResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Tags = new[] {"Admin.Users.Auth"})]
    public async Task<IActionResult> AdminGetAccessTokenAsync([FromServices] IAdminLogAsUseCase useCase, [FromRoute] Guid id)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(id);

        return _viewModel!;
    }
}
