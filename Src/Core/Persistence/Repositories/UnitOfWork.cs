using Domain.Repositories;

namespace Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;

    public UnitOfWork(ApplicationContext context) => _context = context;

    public Task SaveChangesAsync(CancellationToken token = default)
        => _context.SaveChangesAsync(token);
}
