using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.OAuth.LogAs;

public interface IOutputPort
{
    void Ok(UserClaim claim);

    void Forbidden();
    void LogAsFailed(string detail = "No Details.", string title = "Log as failed.", int code = 409_111);
}
