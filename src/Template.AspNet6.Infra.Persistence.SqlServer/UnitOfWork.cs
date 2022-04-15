using Template.AspNet6.Application.Services.Persistence;

namespace Template.AspNet6.Infra.Persistence.SqlServer;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly Context _context;
    private bool _disposed;

    public UnitOfWork(Context context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    /// <inheritdoc />
    public async Task<int> SaveAsync()
    {
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing) _context.Dispose();

        _disposed = true;
    }
}
