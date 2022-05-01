using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Demand
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }

    public Demand(DateTime date, string userId, int shopId)
    {

    }
}
