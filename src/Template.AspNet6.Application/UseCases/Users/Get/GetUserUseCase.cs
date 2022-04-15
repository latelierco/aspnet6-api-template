using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Get;

public class GetUsersUseCase : IGetUsersUseCase
{
    private readonly IReadUserRepository _readUsers;
    private IOutputPort? _outputPort;

    public GetUsersUseCase(IReadUserRepository readUsers) => _readUsers = readUsers;

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(string? search, string[] roles, bool? isActivated, string? orderBy, int pageNumber = 0, int pageSize = 50)
    {
        var users = await _readUsers.GetAsync(search, roles, isActivated, orderBy, pageNumber, pageSize);
        _outputPort?.Ok(users);
    }

    public async Task ExecuteAsync(Guid id)
    {
        var user = await _readUsers.GetAsync(id);
        if (user is null)
        {
            _outputPort?.NotFound();
            return;
        }

        _outputPort?.Ok(user);
    }
}
