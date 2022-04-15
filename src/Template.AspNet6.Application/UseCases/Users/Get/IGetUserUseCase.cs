namespace Template.AspNet6.Application.UseCases.Users.Get;

public interface IGetUsersUseCase
{
    Task ExecuteAsync(string? search, string[] roles, bool? isActivated, string? orderBy, int pageNumber = 0, int pageSize = 50);
    Task ExecuteAsync(Guid id);

    void SetOutputPort(IOutputPort outputPort);
}
