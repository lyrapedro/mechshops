using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Schedule : Entity
{
    public string ShopId { get; set; }
    public DateTime Date { get; set; }

    public Schedule() {}

    public Schedule(DateTime date, string shopId, int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        Guid validGuid;
        Date = date;
        ShopId = shopId;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        Validate(shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
    }

    public void EditInfo(DateTime date, int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary = 0)
    {
        Date = date;
        ModifiedAt = DateTime.UtcNow;

        Validate(shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
    }

    private void Validate(int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        DateTime todayDateConverted = ConvertUtcToLocalDate(DateTime.UtcNow).Date;

        if (Date.Date < todayDateConverted.Date)
            AddNotification("Date", "Cannot schedule for past dates");

        if (IsWeekend(Date))
            AddNotification("Date", "Cannot schedule for weekend");

        if (!HaveEnoughWorkLoad(shopTotalWorkLoad, workLoadUsed, workLoadNecessary))
            AddNotification("WorkLoad", "Does not have enough workload for this day");
    }

    public bool IsWeekend(DateTime date)
    {
        bool isWeekend = (date.DayOfWeek == DayOfWeek.Sunday) || (date.DayOfWeek == DayOfWeek.Saturday);
        return isWeekend;
    }

    public DateTime ConvertUtcToLocalDate(DateTime dateToConvert)
    {
        return TimeZoneInfo.ConvertTime(dateToConvert, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }

    public bool HaveEnoughWorkLoad(int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        if(workLoadNecessary == 0)
            return shopTotalWorkLoad > workLoadUsed;
        else
            return (shopTotalWorkLoad - workLoadUsed) >= workLoadNecessary;
    }
}
