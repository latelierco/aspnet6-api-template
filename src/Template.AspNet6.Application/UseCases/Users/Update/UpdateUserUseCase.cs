using Template.AspNet6.Application.Services.Persistence;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IReadUserRepository _readUsers;
    private readonly IUnitOfWork _uow;
    private readonly IWriteUserRepository _writeUsers;
    private IOutputPort? _outputPort;

    public UpdateUserUseCase(IReadUserRepository readUsers, IWriteUserRepository writeUsers, IUnitOfWork uow)
    {
        _readUsers = readUsers;
        _writeUsers = writeUsers;
        _uow = uow;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(Guid userId, string? firstname, string? lastname)
    {
        try
        {
            var user = await _readUsers.GetAsync(userId);
            if (user is null)
            {
                _outputPort?.NotFound();
                return;
            }

            user.Patch(firstname, lastname);

            _writeUsers.Update(user);
            await _uow.SaveAsync();

            _outputPort?.Ok(user);
        }
        catch (Exception e)
        {
            _outputPort?.UpdateUserFailed(e.Message);
        }
    }
}
