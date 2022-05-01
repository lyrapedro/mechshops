using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Schedules;

public class SchedulePost
{
    public static string Template => "shops/{shopId:int}/schedules";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int shopId, HttpContext http, ScheduleRequest scheduleRequest, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var shop = context.Shops.FirstOrDefault(s => s.Id == shopId);

        if (shop == null)
            return Results.NotFound("Schedule does not exist");

        var schedule = new Schedule(scheduleRequest.Date, userId, shopId);
        if (!schedule.IsValid)
            return Results.ValidationProblem(schedule.Notifications.ConvertToProblemDetails());

        await context.Schedules.AddAsync(schedule);

        await context.SaveChangesAsync();

        return Results.Created($"/schedules/{schedule.Id}", schedule.Id);
    }
}
