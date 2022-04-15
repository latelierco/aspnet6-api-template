using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Update;

public interface IOutputPort
{
    public void Ok(User user);

    void UpdateUserFailed(string detail = "", string title = "L'utilisateur n'a pas pu etre mis a jour", int code = 409_201);
    public void NotFound();
}
