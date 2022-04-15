using System.ComponentModel.DataAnnotations;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Api.UseCases.Me.Update;

public class UpdateMeRequest
{
    [MaxLength(User.FirstNameMaxLength)]
    public string? Firstname { get; set; }

    [MaxLength(User.LastNameMaxLength)]
    public string? Lastname { get; set; }
}
