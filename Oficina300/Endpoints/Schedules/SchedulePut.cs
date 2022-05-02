using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Schedules;

public class SchedulePut
{
    public static string Template => "/schedules/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ScheduleRequest scheduleRequest, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;    
        var schedule = context.Schedules.FirstOrDefault(s => s.Id == id && s.ShopId == shopId);

        if (schedule == null)
            return Results.NotFound("Schedule does not exist");

        schedule.EditInfo(scheduleRequest.Date);

        if (!schedule.IsValid)
            return Results.ValidationProblem(schedule.Notifications.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
