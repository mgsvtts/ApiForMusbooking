using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public sealed class ServiceObjectRepository : IServiceObjectRepository
{
    private readonly ApplicationContext _context;

    public ServiceObjectRepository(ApplicationContext context) => _context = context;

    public void Add(ServiceObject entity)
        => _context.ServiceObjects.Add(entity);

    public void Update(ServiceObject entity)
        => _context.ServiceObjects.Update(entity);

    public void Remove(ServiceObject entity)
        => _context.ServiceObjects.Remove(entity);

    public ServiceObject ExecuteQuery(Func<IQueryable<ServiceObject>, ServiceObject> query)
       => query.Invoke(_context.ServiceObjects);

    public async Task<ServiceObject> ExecuteQuery(Func<IQueryable<ServiceObject>, Task<ServiceObject>> query,
                                                  CancellationToken token = default)
      => await query.Invoke(_context.ServiceObjects);

    public IReadOnlyList<ServiceObject> ExecuteQuery(Func<IQueryable<ServiceObject>, IQueryable<ServiceObject>> query)
        => query.Invoke(_context.ServiceObjects).ToList();

    public async Task<IReadOnlyList<ServiceObject>> ExecuteQuery(Func<IQueryable<ServiceObject>, Task<IQueryable<ServiceObject>>> query,
                                                                 CancellationToken token = default)
       => await (await query.Invoke(_context.ServiceObjects)).ToListAsync(token);
}
