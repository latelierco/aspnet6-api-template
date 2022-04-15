using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Api.ViewModel.Claims;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.OAuth.SignIn
{
    public sealed class SignInResponse
    {
        public SignInResponse(UserClaim claim) => User = new UserClaimViewModel(claim);

        [Required] public UserClaimViewModel User { get; }
    }
}
