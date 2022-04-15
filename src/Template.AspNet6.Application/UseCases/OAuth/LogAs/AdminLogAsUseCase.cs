using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;

namespace Template.AspNet6.Application.UseCases.OAuth.LogAs;

public class AdminLogAsUseCase : IAdminLogAsUseCase
{
    private readonly IAuthenticator _auth;
    private readonly IIdentityProvider _identity;
    private readonly IReadUserRepository _readUsers;
    private IOutputPort? _outputPort;


    public AdminLogAsUseCase(IIdentityProvider identity, IAuthenticator auth, IReadUserRepository readUsers)
    {
        _identity = identity;
        _auth = auth;
        _readUsers = readUsers;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(Guid userId)
    {
        try
        {
            var identity = _identity.GetCurrentIdentity();

            if (identity is null || identity.HasNotRole(CRoles.Admin))
            {
                _outputPort?.Forbidden();
                return;
            }

            var userToLogAs = await _readUsers.GetAsync(userId);
            if (userToLogAs is null)
            {
                _outputPort?.LogAsFailed("User not found");
                return;
            }

            if (userToLogAs.RolesAsEnumerable.Contains(CRoles.Admin) && identity.HasNotRole(CRoles.Admin))
            {
                _outputPort?.Forbidden();
                return;
            }

            var claim = _auth.LogAs(identity, userToLogAs);

            _outputPort?.Ok(claim);
        }
        catch (Exception e)
        {
            _outputPort?.LogAsFailed(e.Message);
        }
    }
}
