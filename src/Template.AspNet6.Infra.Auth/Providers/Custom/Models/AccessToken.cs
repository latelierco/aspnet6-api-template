namespace Template.AspNet6.Infra.Auth.Providers.Custom.Models;

public class AccessToken
{
    public string access_token { get; set; } = null!;
    public string expires_in { get; set; } = null!;
    public string token_type { get; set; } = null!;
    public string scope { get; set; } = null!;
    public string refresh_token { get; set; } = null!;
}
