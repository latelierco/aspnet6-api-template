using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.UseCases.Users.Update;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Users.Update;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UsersController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.UpdateUserFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.NotFound() => _viewModel = NotFound(new ProblemDetails());

    void IOutputPort.Ok(User user) => _viewModel = Ok(new UserResponse(user));

    /// <summary>
    ///     Update user
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = CRoles.Admin)]
    [SwaggerOperation(Tags = new[] {"Admin.Users"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateUserAsync(
        [FromServices] IUpdateUserUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(id, request.Firstname, request.Lastname);

        return _viewModel!;
    }
}
