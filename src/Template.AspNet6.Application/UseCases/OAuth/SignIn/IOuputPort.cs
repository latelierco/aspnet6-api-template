using Template.AspNet6.Domain.Common;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.OAuth.SignIn;

public interface IOutputPort
{
    void Ok(UserClaim claim);

    void UserNotFound(string detail = "", string title = "Sign in failed.", int code = 404_030);

    void SignInFailed(string detail = "", string title = "Sign in failed.", int code = 409_030);

    void InvalidInput(ModelState inputModelState);
}
