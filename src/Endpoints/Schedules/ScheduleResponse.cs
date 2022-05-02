namespace MechShops.Endpoints.Schedules;

public record ScheduleResponse(int Id, string date, DateTime ModifiedAt, DateTime CreatedAt, List<string> services);