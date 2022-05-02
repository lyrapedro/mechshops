using Microsoft.AspNetCore.Authorization;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Schedules;

public class SchedulePost
{
    public static string Template => "shops/schedules";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action(HttpContext http, ScheduleRequest scheduleRequest, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        int shopTotalWorkLoad = Int32.Parse(http.User.Claims.First(c => c.Type == "WorkLoad").Value);

        bool increasedWorkLoad = scheduleRequest.Date.DayOfWeek == DayOfWeek.Thursday || scheduleRequest.Date.DayOfWeek == DayOfWeek.Friday;

        if (increasedWorkLoad)
            shopTotalWorkLoad = shopTotalWorkLoad + (int)(shopTotalWorkLoad * 0.3);

        var workLoadUsed = context.Demands.Where(d => d.Schedule.ShopId == shopId && d.Schedule.Date.Date == scheduleRequest.Date.Date).Sum(s => s.Service.WorkUnits);

        var services = context.Services.Where(s => scheduleRequest.Services.Contains(s.Name) && s.ShopId == shopId).ToList();

        if (!services.Any())
            return Results.NotFound("Services does not exists");

        var workLoadNecessary = services.Sum(s => s.WorkUnits);

        var schedule = new Schedule(scheduleRequest.Date, shopId, shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
        if (!schedule.IsValid)
            return Results.ValidationProblem(schedule.Notifications.ConvertToProblemDetails());

        await context.Schedules.AddAsync(schedule);
        await context.SaveChangesAsync();

        foreach (var service in services)
        {
            var demand = new Demand(service.Id, schedule.Id);
            await context.Demands.AddAsync(demand);
        }

        await context.SaveChangesAsync();

        return Results.Created($"/shops/schedules/{schedule.Id}", schedule.Id);
    }
}
