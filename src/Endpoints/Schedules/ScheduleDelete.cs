using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Schedules;

public class ScheduleDelete
{
    public static string Template => "/shops/schedules/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;

        var schedule = context.Schedules.FirstOrDefault(s => s.Id == id && s.ShopId == shopId);

        if (schedule == null)
            return Results.NotFound("Schedule does not exist");

        var demands = context.Demands.Where(s => s.ScheduleId == schedule.Id).ToList();

        context.Demands.RemoveRange(demands);
        context.Schedules.Remove(schedule);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
