using Template.AspNet6.Application.Services.Persistence;
using Template.AspNet6.Domain.Entities.Users;

namespace Template.AspNet6.Application.UseCases.Users.Update.ProfilePicture;

public class UpdateProfilePictureUseCase : IUpdateProfilePictureUseCase
{
    private readonly IReadUserRepository _readUsers;
    private readonly IStoreService _storeService;
    private readonly IUnitOfWork _uow;
    private readonly IWriteUserRepository _writeUsers;

    private IOutputPort? _outputPort;

    public UpdateProfilePictureUseCase(IReadUserRepository readUsers, IWriteUserRepository writeUsers, IUnitOfWork unitOfWork, IStoreService storeService)
    {
        _readUsers = readUsers;
        _writeUsers = writeUsers;
        _uow = unitOfWork;
        _storeService = storeService;
    }

    public void SetOutputPort(IOutputPort outputPort) => _outputPort = outputPort;

    public async Task ExecuteAsync(Guid id, string filename, MemoryStream logo)
    {
        try
        {
            var user = await _readUsers.GetAsync(id);
            if (user is null)
            {
                _outputPort?.NotFound();
                return;
            }

            var blobUrl = await _storeService.StoreBlobAsync("profile_picture", $"{id}{Path.GetExtension(filename)}", logo, $"{id}");
            user.ProfilePicture = blobUrl;

            _writeUsers.Update(user);
            await _uow.SaveAsync();

            _outputPort?.NoContent();
        }
        catch (Exception e)
        {
            _outputPort?.UpdateProfilePictureFailed(e.Message);
        }
    }
}
