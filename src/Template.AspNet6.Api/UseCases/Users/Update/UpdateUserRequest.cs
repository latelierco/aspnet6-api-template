using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.Users.Update;

public class UpdateUserRequest
{
    [MaxLength(User.FirstNameMaxLength)]
    public string? Firstname { get; set; }

    [MaxLength(User.LastNameMaxLength)]
    public string? Lastname { get; set; }
}
