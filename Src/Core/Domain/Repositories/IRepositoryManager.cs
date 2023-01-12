namespace Domain.Repositories;

public interface IRepositoryManager
{
    public IServiceObjectRepository ServiceObjectRepository { get; }

    public IUnitOfWork UnitOfWork { get; }
}
