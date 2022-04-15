using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.OAuth.RefreshToken;

public interface IOutputPort
{
    void Ok(UserClaim claim);
    void RefreshTokenFailed(string detail = "No Details.", string title = "Refresh token failed.", int code = 409_041);
}
