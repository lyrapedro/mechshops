namespace MechShops.Domain.Shops;

public class Schedule : Entity
{
    public string ShopId { get; set; }
    public DateTime Date { get; set; }

    public Schedule() {}

    public Schedule(DateTime date, string shopId, int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        Date = date;
        ShopId = shopId;
        ModifiedAt = DateTime.Now;
        CreatedAt = DateTime.Now;

        Validate(shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
    }

    public void EditInfo(DateTime date, int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary = 0)
    {
        Date = date;
        ModifiedAt = DateTime.Now;

        Validate(shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
    }

    private void Validate(int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        DateTime todayDate = DateTime.Now;

        if (Date.Date < todayDate.Date)
            AddNotification("Date", "Cannot schedule for past dates");

        if (IsWeekend(Date))
            AddNotification("Date", "Cannot schedule for weekend");

        if (NoHaveEnoughWorkLoad(shopTotalWorkLoad, workLoadUsed, workLoadNecessary))
            AddNotification("WorkLoad", "Does not have enough workload for this day");
    }

    public bool IsWeekend(DateTime date)
    {
        bool isWeekend = (date.DayOfWeek == DayOfWeek.Sunday) || (date.DayOfWeek == DayOfWeek.Saturday);
        return isWeekend;
    }

    public bool NoHaveEnoughWorkLoad(int shopTotalWorkLoad, int workLoadUsed, int workLoadNecessary)
    {
        if(workLoadNecessary == 0)
            return shopTotalWorkLoad > workLoadUsed;
        else
            return !((shopTotalWorkLoad - workLoadUsed) >= workLoadNecessary);
    }
}
