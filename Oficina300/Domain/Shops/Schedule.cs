using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Schedule : Entity
{
    public string ShopId { get; set; }
    public DateTime Date { get; set; }

    public Schedule() {}

    public Schedule(DateTime date, string shopId, int shopTotalWorkLoad, int workLoadUsed)
    {
        Guid validGuid;
        Date = date;
        ShopId = shopId;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        Validate(shopTotalWorkLoad, workLoadUsed);
    }

    public void EditInfo(DateTime date, int shopTotalWorkLoad, int workLoadUsed)
    {
        Date = date;
        ModifiedAt = DateTime.UtcNow;

        Validate(shopTotalWorkLoad, workLoadUsed);
    }

    private void Validate(int shopTotalWorkLoad, int workLoadUsed)
    {
        DateTime todayDateConverted = ConvertUtcToLocalDate(DateTime.UtcNow).Date;

        var contract = new Contract<Schedule>()
            .IsGreaterOrEqualsThan(Date.Date, todayDateConverted, "Date")
            .IsTrue(haveEnoughWorkLoad(shopTotalWorkLoad, workLoadUsed), "Date");
        AddNotifications(contract);
    }

    public DateTime ConvertUtcToLocalDate(DateTime dateToConvert)
    {
        return TimeZoneInfo.ConvertTime(dateToConvert, TimeZoneInfo.FindSystemTimeZoneById("Central Brazilian Standard Time"));
    }

    public bool haveEnoughWorkLoad(int shopTotalWorkLoad, int workLoadUsed)
    {
        return shopTotalWorkLoad > workLoadUsed;
    }
}
