namespace Oficina300.Endpoints.Schedules;

public record ScheduleRequest(DateTime Date, List<string> Services);