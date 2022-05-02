using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Shops;

public class GetWorkLoadByDay
{
    public static string Template => "shops/workload";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static IResult Action(HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var workLoad = Int32.Parse(http.User.Claims.First(c => c.Type == "WorkLoad").Value);

        var currentDate = DateTime.Now;
        var maxDate = currentDate.AddDays(5);

        var schedules = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        schedules = schedules.Where(s => s.Date.Date >= currentDate.Date && s.Date.Date <= maxDate.Date).ToList();

        var schedulesIds = schedules.Select(s => s.Id).ToList();

        var demands = context.Demands.Include(d => d.Schedule).Include(d => d.Service).Where(d => schedulesIds.Contains(d.ScheduleId)).ToList();

        var schedulesDates = schedules.GroupBy(s => s.Date.Date);

        List<WorkLoadResponse> response = new List<WorkLoadResponse>();

        foreach (var date in schedulesDates)
        {
            var workLoadUsed = demands.Where(d => d.Schedule.Date.Date == date.Key).Sum(s => s.Service.WorkUnits);
            var workLoadAvailable = (workLoad - workLoadUsed) >= 0 ? (workLoad - workLoadUsed) : 0;
            var obj = new WorkLoadResponse(date.Key.ToString("dd/MM/yy"), workLoadAvailable, workLoadUsed);
            response.Add(obj);
        }

        response = response.OrderBy(r => r.Date).ToList();

        return Results.Ok(response);
    }
}
