using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Add;

public interface IOutputPort
{
    public void Ok(User user);
    void CreateUserFailed(string detail = "", string title = "L'utilisateur n'a pas pu etre ajoute", int code = 409_200);
}
