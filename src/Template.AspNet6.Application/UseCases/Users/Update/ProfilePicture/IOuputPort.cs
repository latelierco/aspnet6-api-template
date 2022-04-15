using Template.AspNet6.Domain.Common;

namespace Template.AspNet6.Application.UseCases.Users.Update.ProfilePicture;

public interface IOutputPort
{
    void NoContent();
    void NotFound();

    void UpdateProfilePictureFailed(string detail, string title = "Update profile picture failed.", int code = 409_001);
    void Invalid(ModelState modelState);
}
