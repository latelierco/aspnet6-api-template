namespace Template.AspNet6.Infra.Auth.Providers.Microsoft.Models;

public class AccessToken
{
    public string access_token { get; set; } = null!;
    public int expires_in { get; set; }
    public string token_type { get; set; } = null!;
    public string scope { get; set; } = null!;
    public string refresh_token { get; set; } = null!;
    public string id_token { get; set; } = null!;
}
