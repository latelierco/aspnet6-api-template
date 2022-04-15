namespace Template.AspNet6.Application.UseCases.Users.Add;

public interface IAddUserUseCase
{
    Task ExecuteAsync(string firstname, string lastname, string email);

    void SetOutputPort(IOutputPort outputPort);
}
