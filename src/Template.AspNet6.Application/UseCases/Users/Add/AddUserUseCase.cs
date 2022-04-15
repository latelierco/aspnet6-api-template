using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Application.Services.Persistence;

namespace Template.AspNet6.Application.UseCases.Users.Add;

public class AddUserUseCase : IAddUserUseCase
{
    private readonly IUserFactory _userFactory;
    private readonly IWriteUserRepository _writeUsers;
    private readonly IUnitOfWork _uow;
    private IOutputPort? _outputPort;

    public AddUserUseCase(IUserFactory userFactory, IWriteUserRepository writeUsers, IUnitOfWork uow)
    {
        _userFactory = userFactory;
        _writeUsers = writeUsers;
        _uow = uow;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(string firstname, string lastname, string email)
    {
        try
        {
            var newUser = _userFactory.NewUser(firstname, lastname, new Email(email));
            
            await _writeUsers.AddAsync(newUser);
            await _uow.SaveAsync();

            _outputPort?.Ok(newUser);
        }
        catch (Exception e)
        {
            _outputPort?.CreateUserFailed(e.Message);
        }
    }
}
