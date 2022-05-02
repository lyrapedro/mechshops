using Microsoft.AspNetCore.Authorization;
using MechShops.Domain.Shops;
using MechShops.Infra.Data;
using System.Globalization;

namespace MechShops.Endpoints.Schedules;

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

        DateTime validDate;
        CultureInfo provider = new CultureInfo("pt-BR");
        DateTime scheduleDate = DateTime.TryParse(scheduleRequest.Date, provider, DateTimeStyles.None, out validDate) ? validDate : DateTime.Now;

        bool increasedWorkLoad = scheduleDate.DayOfWeek == DayOfWeek.Thursday || scheduleDate.DayOfWeek == DayOfWeek.Friday;

        if (increasedWorkLoad)
            shopTotalWorkLoad = shopTotalWorkLoad + (int)(shopTotalWorkLoad * 0.3);

        var workLoadUsed = context.Demands.Where(d => d.Schedule.ShopId == shopId && d.Schedule.Date.Date == scheduleDate.Date).Sum(s => s.Service.WorkUnits);

        var services = context.Services.Where(s => scheduleRequest.Services.Contains(s.Name) && s.ShopId == shopId).ToList();

        if (!services.Any())
            return Results.NotFound("Services does not exists");

        var workLoadNecessary = services.Sum(s => s.WorkUnits);

        var schedule = new Schedule(scheduleDate, shopId, shopTotalWorkLoad, workLoadUsed, workLoadNecessary);
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
