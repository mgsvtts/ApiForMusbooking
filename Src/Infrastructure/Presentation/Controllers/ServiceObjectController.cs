using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Text.Json;

namespace Presentation.Controllers;

[ApiController]
public class ServiceObjectController : ControllerBase
{
    private readonly IServiceObjectService _service;

    public ServiceObjectController(IServiceObjectService service) => _service = service;

    [HttpPost]
    [Route("api/services/create")]
    public async Task<string> Create(string name, int amount, CancellationToken token)
    {
        var created = await _service.CreateAsync(name, amount, token);

        return JsonSerializer.Serialize(new { id = created.Id });
    }

    [HttpPut]
    [Route("api/services/update")]
    public async Task<string> Update(Guid id, string? name, int? amount, CancellationToken token)
    {
        var updated = await _service.UpdateAsync(id, name, amount, token);

        return JsonSerializer.Serialize(new { id = updated.Id });
    }

    [HttpPost]
    [Route("api/services/booking")]
    public async Task<string> Booking(Guid id, int amount, CancellationToken token)
    {
        var result = await _service.BookingAsync(id, amount, token);

        return JsonSerializer.Serialize(new
        {
            ok = result.Ok,
            amount = result.AmountLeft,
            error = result.Error
        });
    }
}
