namespace Template.AspNet6.Domain.Entities.Users;

public interface IWriteUserRepository
{
    Task AddAsync(User user);
    void Update(User user);
}
