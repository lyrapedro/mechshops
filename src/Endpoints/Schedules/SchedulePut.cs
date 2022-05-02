using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechShops.Infra.Data;
using System.Globalization;
using System.Security.Claims;

namespace MechShops.Endpoints.Schedules;

public class SchedulePut
{
    public static string Template => "/schedules/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ScheduleRequest scheduleRequest, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        int shopTotalWorkLoad = Int32.Parse(http.User.Claims.First(c => c.Type == "WorkLoad").Value);

        DateTime validDate;
        CultureInfo provider = new CultureInfo("pt-BR");
        DateTime scheduleDate = DateTime.TryParse(scheduleRequest.Date, provider, DateTimeStyles.None, out validDate) ? validDate : DateTime.Now;

        bool increasedWorkLoad = scheduleDate.DayOfWeek == DayOfWeek.Thursday || scheduleDate.DayOfWeek == DayOfWeek.Friday;

        if (increasedWorkLoad)
            shopTotalWorkLoad = shopTotalWorkLoad + (int)(shopTotalWorkLoad * 0.3);

        var schedule = context.Schedules.FirstOrDefault(s => s.Id == id && s.ShopId == shopId);

        if (schedule == null)
            Results.NotFound("Schedule dos not exists");

        var shopDemands = context.Demands.Where(d => d.Schedule.ShopId == shopId).ToList();

        var scheduleWorkUnits = shopDemands.Where(d => d.ScheduleId == schedule.Id).Sum(d => d.Service.WorkUnits);

        int workLoadUsed;
        if (scheduleDate.Date == schedule.Date.Date)
            workLoadUsed = scheduleWorkUnits * (-1);

        workLoadUsed =+ shopDemands.Where(d => d.Schedule.ShopId == shopId && d.Schedule.Date.Date == scheduleDate.Date).Sum(s => s.Service.WorkUnits);

        schedule.EditInfo(scheduleDate, shopTotalWorkLoad, workLoadUsed);

        if (!schedule.IsValid)
            return Results.ValidationProblem(schedule.Notifications.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
