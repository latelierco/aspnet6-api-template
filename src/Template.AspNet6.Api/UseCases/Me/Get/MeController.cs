using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Api.UseCases.Users;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Application.UseCases.Users.Get;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Me.Get;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MeController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.NotFound(string detail, string title, int code) => _viewModel = NotFound(new ProblemDetails {Detail = detail, Title = title, Instance = $"{code}"});

    void IOutputPort.Ok((int count, IEnumerable<User> items) users)
    {
        Response.Headers.Add("Access-Control-Expose-Headers", Constants.ExposedPaginationHeaderName);
        Response.Headers.Add(Constants.ExposedPaginationHeaderName, $"{users.count}");
        _viewModel = Ok(new UsersResponse(users.items));
    }

    void IOutputPort.Ok(User user) => _viewModel = Ok(new UserResponse(user));

    /// <summary>
    ///     Get authenticated user
    /// </summary>
    [HttpGet]
    [Authorize(Roles = CRoles.User)]
    [SwaggerOperation(Tags = new[] {"Me"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetMeAsync(
        [FromServices] IIdentityProvider identityProvider,
        [FromServices] IGetUsersUseCase useCase)
    {
        useCase.SetOutputPort(this);

        var identity = identityProvider.GetCurrentIdentity();

        await useCase.ExecuteAsync(identity!.UserId);

        return _viewModel!;
    }
}
