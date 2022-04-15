using Template.AspNet6.Domain.Entities.Users.Claims;
using Microsoft.EntityFrameworkCore;

namespace Template.AspNet6.Infra.Persistence.SqlServer.Users.Claims;

public class ClaimRepository : IClaimWriteRepository, IClaimReadRepository
{
    private readonly Context _context;

    public ClaimRepository(Context context)
    {
        _context = context;
    }

    public Task<string?> GetClaimValueAsync(Guid userId, string provider)
    {
        return _context.Claims
            .Where(x => x.UserId == userId && x.Provider == provider)
            .Select(x => x.Value)
            .FirstOrDefaultAsync();
    }

    public Task<Claim?> GetClaimAsync(Guid userId, string provider)
    {
        return _context.Claims
            .Include(x => x.User)
            .OrderByDescending(x => x.ExpirationDate)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider);
    }

    public Task<Claim?> GetClaimAsync(string value, string provider)
    {
        return _context.Claims
            .Include(x => x.User)
            .OrderByDescending(x => x.ExpirationDate)
            .FirstOrDefaultAsync(x => x.Value == value && x.Provider == provider);
    }

    public async Task AddClaimAsync(Claim claim)
    {
        await _context.Claims
            .AddAsync(claim);
    }

    public void RemoveClaim(Claim claim)
    {
        _context.Claims
            .Remove(claim);
    }

    public void RemoveAllClaimsAsync(Guid userId, string provider)
    {
        var claimsToRevoke = _context.Claims
            .Where(x => x.UserId == userId && x.Provider == provider);
        foreach (var claim in claimsToRevoke)
            claim.ExpirationDate = DateTime.UtcNow;

        _context.Claims.UpdateRange(claimsToRevoke);
    }

    public void UpdateClaim(Claim claimToUpdate)
    {
        _context.Claims
            .Update(claimToUpdate);
    }
}
