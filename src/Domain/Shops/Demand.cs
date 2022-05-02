namespace MechShops.Domain.Shops;

public class Demand
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }

    public Demand(int serviceId, int scheduleId)
    {
        ScheduleId = scheduleId;
        ServiceId = serviceId;
    }
}
