namespace Template.AspNet6.Infra.Auth.Providers.Microsoft.Models;

public class UserProviderModel
{
    public Guid Id { get; set; }
    public List<string> BusinessPhones { get; set; } = new();
    public string DisplayName { get; set; } = null!;
    public string GivenName { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Mail { get; set; } = null!;
    public string MobilePhone { get; set; } = null!;
    public string OfficeLocation { get; set; } = null!;
    public string PreferredLanguage { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string UserPrincipalName { get; set; } = null!;
}
