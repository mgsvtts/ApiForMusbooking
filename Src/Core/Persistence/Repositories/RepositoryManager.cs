using Domain.Repositories;

namespace Persistence.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IServiceObjectRepository> _lazyServiceObjectRepository;
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

    public RepositoryManager(ApplicationContext context)
    {
        _lazyServiceObjectRepository = new Lazy<IServiceObjectRepository>(() => new ServiceObjectRepository(context));
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(context));
    }

    public IServiceObjectRepository ServiceObjectRepository => _lazyServiceObjectRepository.Value;
    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
}
