using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Schedule : Entity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; }
    public DateTime Date { get; set; }
    public Guid CreatedBy { get; set; }


    public Schedule(DateTime date, Guid createdBy)
    {
        Date = date;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        ModifiedBy = Guid.Empty;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Schedule>()
            .IsGreaterOrEqualsThan(Date, DateTime.UtcNow, "Date");
        AddNotifications(contract);
    }

    public void EditInfo(DateTime date, string modifiedBy)
    {
        Guid validGuid;
        Date = date;
        ModifiedBy = Guid.TryParse(modifiedBy, out validGuid) ? validGuid : Guid.Empty;
        ModifiedAt = DateTime.UtcNow;

        Validate();
    }
}
