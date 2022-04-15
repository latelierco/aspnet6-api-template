using System.ComponentModel.DataAnnotations;

namespace Template.AspNet6.Api.UseCases.Me.Update.ProfilePicture;

public class UpdateProfilePictureRequest
{
    [Required] public IFormFile File { get; set; } = null!;
}