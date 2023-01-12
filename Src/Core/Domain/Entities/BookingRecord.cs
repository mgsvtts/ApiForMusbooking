namespace Domain.Entities;

public sealed class BookingRecord
{
    public string ObjectName { get; set; }

    public int Amount { get; set; }

    public DateTime CreatedDate { get; set; }
}
