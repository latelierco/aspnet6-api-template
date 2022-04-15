using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Get;

public interface IOutputPort
{
    public void Ok((int count, IEnumerable<User> items) users);
    public void Ok(User user);

    void NotFound(string detail = "", string title = "Get user failed", int code = 404_001);
}
