namespace Template.AspNet6.Application.UseCases.Users.Update.ProfilePicture;

public interface IUpdateProfilePictureUseCase
{
    Task ExecuteAsync(Guid id, string filename, MemoryStream logo);

    void SetOutputPort(IOutputPort outputPort);
}
