using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Schedule : Entity
{
    public string ShopId { get; set; }
    public DateTime Date { get; set; }

    public Schedule()
    {
        Validate();
    }

    public Schedule(DateTime date, string shopId)
    {
        Guid validGuid;
        Date = date;
        ShopId = shopId;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Schedule>()
            .IsGreaterOrEqualsThan(Date, DateTime.UtcNow, "Date");
        AddNotifications(contract);
    }

    public void EditInfo(DateTime date)
    {
        Date = date;
        ModifiedAt = DateTime.UtcNow;

        Validate();
    }
}
