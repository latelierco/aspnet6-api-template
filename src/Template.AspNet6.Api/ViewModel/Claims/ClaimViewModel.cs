using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Api.ViewModel.Users;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.ViewModel.Claims;

public class UserClaimViewModel : UserViewModel
{
    public UserClaimViewModel(UserClaim claim) : base(claim.User) => Claim = new ClaimViewModel(claim.AccessToken.Token, claim.AccessToken.ExpirationDate, claim.RefreshToken.Token, claim.RefreshToken.ExpirationDate);

    public ClaimViewModel Claim { get; set; }

    public class ClaimViewModel
    {
        public ClaimViewModel(string accessToken, DateTime accessTokenExpirationDate, string refreshToken, DateTime refreshTokenExpirationDate)
        {
            AccessToken = accessToken;
            AccessTokenExpirationDate = accessTokenExpirationDate;

            RefreshToken = refreshToken;
            RefreshTokenExpirationDate = refreshTokenExpirationDate;
        }

        [Required] public string AccessToken { get; set; }
        [Required] public DateTime AccessTokenExpirationDate { get; set; }

        [Required] public string RefreshToken { get; set; }
        [Required] public DateTime RefreshTokenExpirationDate { get; set; }
    }
}