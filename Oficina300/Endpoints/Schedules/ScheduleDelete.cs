using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Schedules;

public class ScheduleDelete
{
    public static string Template => "/shops/{shopId:int}/schedules/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int shopId, [FromRoute] int id, HttpContext http, ApplicationDbContext context)
    {
        var userShopIds = http.User.Claims.Where(c => c.Type == "ShopId").Select(c => c.Value);
        var isShopEmployee = userShopIds.Contains(shopId.ToString());

        if (!isShopEmployee)
            return Results.Unauthorized(); 

        var schedule = context.Schedules.FirstOrDefault(s => s.Id == id);

        if (schedule == null)
            return Results.NotFound("Schedule does not exist");

        var schedules = context.Schedules.Where(s => s.ShopId == schedule.Id).ToList();

        if (schedules.Any())
            context.Schedules.RemoveRange(schedules);

        context.Schedules.Remove(schedule);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
