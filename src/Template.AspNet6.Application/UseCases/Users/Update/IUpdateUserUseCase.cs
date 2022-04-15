namespace Template.AspNet6.Application.UseCases.Users.Update;

public interface IUpdateUserUseCase
{
    Task ExecuteAsync(Guid userId, string? firstname, string? lastname);

    void SetOutputPort(IOutputPort outputPort);
}
