namespace MechShops.Endpoints.Schedules;

public record ScheduleRequest(string Date, List<string> Services);