namespace Template.AspNet6.Application.UseCases.OAuth.LogAs;

public interface IAdminLogAsUseCase
{
    Task ExecuteAsync(Guid userId);
    void SetOutputPort(IOutputPort outputPort);
}
