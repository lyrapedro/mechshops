namespace Oficina300.Endpoints.Schedules;

public record ScheduleResponse(int Id, int ShopId, Guid CreatedBy, DateTime ModifiedAt, DateTime CreatedAt);