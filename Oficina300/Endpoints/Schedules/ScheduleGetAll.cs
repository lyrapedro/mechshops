using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Schedules;

public class ScheduleGetAll
{
    public static string Template => "shops/schedules";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static IResult Action(HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var schedule = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        var response = schedule.Select(s => new ScheduleResponse(s.Id, s.ModifiedAt, s.CreatedAt));

        return Results.Ok(response);
    }
}
