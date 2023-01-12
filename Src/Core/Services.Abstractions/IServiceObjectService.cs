using Contracts;

namespace Services.Abstractions;

public interface IServiceObjectService
{
    public Task<ServiceObjectDto> CreateAsync(string name, int amount, CancellationToken token = default);

    public Task<ServiceObjectDto> UpdateAsync(Guid id, string? name, int? amount, CancellationToken token = default);

    public Task<(bool Ok, int AmountLeft, string Error)> BookingAsync(Guid id, int amount, CancellationToken token = default);
}
