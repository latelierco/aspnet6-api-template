namespace Template.AspNet6.Api.UseCases.Users.Get;

public class GetUserRequest
{
    public string? Search { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
    public bool? IsActivated { get; set; }
    public string? SortBy { get; set; }
}
