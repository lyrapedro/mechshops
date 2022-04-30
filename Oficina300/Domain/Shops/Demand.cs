namespace Oficina300.Domain.Shops;

public class Demand
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int ShopId { get; set; }
    public Shop Shop { get; set; }
}
