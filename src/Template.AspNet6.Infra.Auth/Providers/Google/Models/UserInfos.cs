namespace Template.AspNet6.Infra.Auth.Providers.Google.Models;

public class UserInfos
{
    public string id { get; set; } = null!;
    public string email { get; set; } = null!;
    public string given_name { get; set; } = null!;
    public string family_name { get; set; } = null!;
    public string picture { get; set; } = null!;
}
