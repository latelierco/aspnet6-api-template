using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.Services.Authentication;

public interface IIdentityProvider
{
    Identity? GetCurrentIdentity();
}
