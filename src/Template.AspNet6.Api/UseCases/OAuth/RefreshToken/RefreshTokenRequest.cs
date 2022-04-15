namespace Template.AspNet6.Api.UseCases.OAuth.RefreshToken;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
