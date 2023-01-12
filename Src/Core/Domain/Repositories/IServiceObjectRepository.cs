using Domain.Entities;

namespace Domain.Repositories;

public interface IServiceObjectRepository
{
    public void Add(ServiceObject entity);

    public void Update(ServiceObject entity);

    public void Remove(ServiceObject entity);

    public ServiceObject ExecuteQuery(Func<IQueryable<ServiceObject>, ServiceObject> query);

    public Task<ServiceObject> ExecuteQuery(Func<IQueryable<ServiceObject>, Task<ServiceObject>> query,
                                            CancellationToken token = default);

    public IReadOnlyList<ServiceObject> ExecuteQuery(Func<IQueryable<ServiceObject>, IQueryable<ServiceObject>> query);

    public Task<IReadOnlyList<ServiceObject>> ExecuteQuery(Func<IQueryable<ServiceObject>, Task<IQueryable<ServiceObject>>> query,
                                                           CancellationToken token = default);
}
