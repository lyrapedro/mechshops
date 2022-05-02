using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Schedules;

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

        List<ScheduleResponse> response = new List<ScheduleResponse>();

        var demands = context.Demands.Include(d => d.Service).Where(d => schedulesIds.Contains(d.ScheduleId)).ToList();

        if(demands.Any())
            response = schedules.Select(s => new ScheduleResponse(s.Id, s.Date.Date.ToString("dd/MM/yy"), s.ModifiedAt, s.CreatedAt,
                demands.Where(d => d.ScheduleId == s.Id).Select(d => d.Service.Name).ToList())).ToList();

        response.OrderBy(r => r.date).ToList();

        return Results.Ok(response);
    }
}
