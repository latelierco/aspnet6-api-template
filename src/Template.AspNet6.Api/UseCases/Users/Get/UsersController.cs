using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.UseCases.Users.Get;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Users.Get;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UsersController : ControllerBase, IOutputPort
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
    ///     Get users
    /// </summary>
    [HttpGet]
    [Authorize(Roles = CRoles.Admin)]
    [SwaggerOperation(Tags = new[] {"Admin.Users"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersResponse))]
    public async Task<IActionResult> GetUsers(
        [FromServices] IGetUsersUseCase useCase,
        [FromQuery] GetUserRequest request,
        [FromHeader(Name = Constants.PageNumberHeader)] int pageNumber = 0,
        [FromHeader(Name = Constants.PageSizeHeader)] int pageSize = 50)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(request.Search, request.Roles, request.IsActivated, request.SortBy, pageNumber, pageSize);

        return _viewModel!;
    }

    /// <summary>
    ///     Get user from unique id
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = CRoles.Admin)]
    [SwaggerOperation(Tags = new[] {"Admin.Users"})]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersResponse))]
    public async Task<IActionResult> GetUser(
        [FromServices] IGetUsersUseCase useCase,
        [FromRoute] Guid id)
    {
        useCase.SetOutputPort(this);

        await useCase.ExecuteAsync(id);

        return _viewModel!;
    }
}
