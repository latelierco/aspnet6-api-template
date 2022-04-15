using Template.AspNet6.Api.ViewModel.Users;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.Users;

public class UserResponse
{
    public UserResponse(User u) => User = new UserViewModel(u);

    public UserViewModel User { get; set; }
}

public class UsersResponse
{
    public UsersResponse(IEnumerable<User> users) => Users = users.Select(u => new UserViewModel(u));

    public IEnumerable<UserViewModel> Users { get; set; }
}