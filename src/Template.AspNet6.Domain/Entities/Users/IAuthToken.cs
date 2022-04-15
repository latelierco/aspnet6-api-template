namespace Template.AspNet6.Domain.Entities.Users;

public interface IAuthToken
{
    string Token { get; set; }
    DateTime ExpirationDate { get; set; }
}
