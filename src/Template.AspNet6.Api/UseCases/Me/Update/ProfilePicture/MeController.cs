using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Application.UseCases.Users.Update.ProfilePicture;
using Template.AspNet6.Domain.Common;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Api.UseCases.Me.Update.ProfilePicture;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public sealed class MeController : ControllerBase, IOutputPort
{
    private IActionResult? _viewModel;

    void IOutputPort.UpdateProfilePictureFailed(string detail, string title, int code) => _viewModel = Conflict(new ProblemDetails {Title = title, Detail = detail, Instance = $"{code}"});
    void IOutputPort.Invalid(ModelState modelState) => _viewModel = BadRequest(new ValidationProblemDetails(modelState.ErrorMessages));
    void IOutputPort.NotFound() => _viewModel = NotFound(new ProblemDetails());
    void IOutputPort.NoContent() => _viewModel = NoContent();


    /// <summary>
    ///     Update profile picture of authenticated user
    /// </summary>
    [HttpPut("picture")]
    [Authorize(Roles = CRoles.User)]
    [SwaggerOperation(Tags = new[] {"Me"})]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateProfilePictureAsync(
        [FromServices] IIdentityProvider identityProvider,
        [FromServices] IUpdateProfilePictureUseCase useCase,
        [Required] [FromForm] UpdateProfilePictureRequest request)
    {
        useCase.SetOutputPort(this);

        var identity = identityProvider.GetCurrentIdentity();

        var memStream = new MemoryStream();
        await request.File.CopyToAsync(memStream);

        await useCase.ExecuteAsync(identity!.UserId, request.File.FileName, memStream);

        return _viewModel!;
    }
}
