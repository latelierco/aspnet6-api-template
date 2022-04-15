using Template.AspNet6.Api.ViewModel.Claims;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.OAuth;

public sealed class AuthResponse
{
    public AuthResponse(UserClaim claim)
    {
        UserClaim = new UserClaimViewModel(claim);
    }

    public UserClaimViewModel UserClaim { get; }
}
