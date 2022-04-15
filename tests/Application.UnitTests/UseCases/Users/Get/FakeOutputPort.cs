using System.Collections.Generic;
using Template.AspNet6.Application.UseCases.Users.Get;
using Template.AspNet6.Domain.Entities.Users;

namespace Application.UnitTests.UseCases.Users.Get;

public class FakeOutputPort : IOutputPort
{
    public IEnumerable<User> Users { get; set; } = new List<User>();
    public User? User { get; set; }
    
    public bool IsOk { get; set; }
    public bool IsNotFound { get; set; }

    void IOutputPort.Ok((int count, IEnumerable<User> items) users)
    {
        Users = users.items;
        IsOk = true;
    }

    void IOutputPort.Ok(User user)
    {
        User = user;
        IsOk = true;
    }

    public void NotFound(string detail = "", string title = "Get user failed", int code = 404001)
    {
        IsNotFound = true;
    }
}
