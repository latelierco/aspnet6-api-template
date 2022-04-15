namespace Template.AspNet6.Application.Services.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveAsync();
}