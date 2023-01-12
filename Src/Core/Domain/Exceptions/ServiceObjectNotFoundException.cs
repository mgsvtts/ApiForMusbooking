namespace Domain.Exceptions;

public sealed class ServiceObjectNotFoundException : NotFoundException
{
    public ServiceObjectNotFoundException(Guid id)
        : base($"Service object with id:{id} not found")
    {
    }
}
