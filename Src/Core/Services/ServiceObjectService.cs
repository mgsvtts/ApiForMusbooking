using Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;

namespace Services;

public sealed class ServiceObjectService : IServiceObjectService
{
    private readonly IRepositoryManager _repositoryManager;
    private static List<BookingRecord> _records = new();

    public ServiceObjectService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<ServiceObjectDto> CreateAsync(string name, int amount, CancellationToken token = default)
    {
        ValidateAmount(amount);

        var item = new ServiceObject
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Amount = amount
        };

        _repositoryManager.ServiceObjectRepository.Add(item);

        await _repositoryManager.UnitOfWork.SaveChangesAsync(token);

        return item.Adapt<ServiceObjectDto>();
    }

    public async Task<ServiceObjectDto> UpdateAsync(Guid id, string? name, int? amount, CancellationToken token = default)
    {
        ValidateAmount(amount);

        var item = await GetServiceObjectAsync(id, token);

        item.Name = name ?? item.Name;
        item.Amount = amount ?? item.Amount;

        await _repositoryManager.UnitOfWork.SaveChangesAsync(token);

        return item.Adapt<ServiceObjectDto>();
    }

    public async Task<(bool Ok, int AmountLeft, string Error)> BookingAsync(Guid id, int amount, CancellationToken token = default)
    {
        var ok = true;
        var item = await GetServiceObjectAsync(id, token);
        var amountLeft = item.Amount - _records.Sum(x => x.Amount);

        if (amount > amountLeft)
        {
            ok = false;
            return (ok, amountLeft, "Requested amount is more than available");
        }

        _records.Add(new BookingRecord
        {
            Amount = amount,
            ObjectName = item.Name,
            CreatedDate = DateTime.Now
        });

        return (ok, item.Amount - _records.Sum(x => x.Amount), string.Empty);
    }

    private async Task<ServiceObject> GetServiceObjectAsync(Guid id, CancellationToken token)
    {
        var item = await _repositoryManager.ServiceObjectRepository.ExecuteQuery(async x
                 => await x.SingleOrDefaultAsync(item => item.Id == id.ToString().ToLower(), token), token);

        return item == null ? throw new ServiceObjectNotFoundException(id) : item;
    }

    private static void ValidateAmount(int? amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount must be >= 0");
        }
    }

    public static IEnumerable<BookingRecord> GetAllRecords() => _records;

    public static void ResetRecords() => _records = new();
}
