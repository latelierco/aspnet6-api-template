using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.UseCases.Users.Add;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Users.Add;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UsersController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.CreateUserFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});

    void IOutputPort.Ok(User user) => _viewModel = Ok(new UserResponse(user));

    /// <summary>
    ///     Create user
    /// </summary>
    [HttpPost]
    [Authorize(Roles = CRoles.Admin)]
    [SwaggerOperation(Tags = new[] {"Admin.Users"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> AddUserAsync([FromServices] IAddUserUseCase useCase, [FromBody] AddUserRequest body)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(body.Firstname, body.Lastname, body.Email);

        return _viewModel!;
    }
}
