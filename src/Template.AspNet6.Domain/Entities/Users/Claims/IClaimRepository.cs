namespace Template.AspNet6.Domain.Entities.Users.Claims;

public interface IClaimWriteRepository
{
    Task AddClaimAsync(Claim claim);
    void RemoveClaim(Claim claim);
    void RemoveAllClaimsAsync(Guid userId, string provider);
    void UpdateClaim(Claim claimToUpdate);
}

public interface IClaimReadRepository
{
    Task<string?> GetClaimValueAsync(Guid userId, string provider);
    Task<Claim?> GetClaimAsync(Guid userId, string provider);
    Task<Claim?> GetClaimAsync(string token, string provider);
}
