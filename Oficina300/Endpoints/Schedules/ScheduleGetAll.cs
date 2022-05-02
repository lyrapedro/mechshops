using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        var currentDate = DateTime.Now;
        var maxDate = currentDate.AddDays(5);

        var schedules = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        schedules = schedules.Where(s => s.Date.Date >= currentDate.Date && s.Date.Date <= maxDate.Date).ToList();

        var schedulesIds = schedules.Select(s => s.Id).ToList();

        var demands = context.Demands.Include(d => d.Service).Where(d => schedulesIds.Contains(d.ScheduleId)).ToList();

        var response = schedules.Select(s => new ScheduleResponse(s.Id, s.Date.Date.ToString("dd/MM/yy"), s.ModifiedAt, s.CreatedAt,
            demands.Where(d => d.ScheduleId == s.Id).Select(d => d.Service.Name).ToList()));

        response = response.OrderBy(r => r.date).ToList();

        return Results.Ok(response);
    }
}
