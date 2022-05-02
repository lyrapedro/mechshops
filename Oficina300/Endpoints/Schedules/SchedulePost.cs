using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

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

        var schedule = new Schedule(scheduleRequest.Date, shopId);
        if (!schedule.IsValid)
            return Results.ValidationProblem(schedule.Notifications.ConvertToProblemDetails());

        var services = context.Services.Where(s => scheduleRequest.Services.Contains(s.Name) && s.ShopId == shopId).ToList();

        if (!services.Any())
            return Results.NotFound("Services does not exists");

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
