namespace Oficina300.Endpoints.Schedules;

public record ScheduleRequest(string Date, List<string> Services);