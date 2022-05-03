using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Shops;

public class GetWorkLoadByDayOrInterval
{
    public static string Template => "/workload";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static IResult Action([FromQuery] string startDate, [FromQuery] string endDate, HttpContext http, ApplicationDbContext context)
    {
        DateTime? validStartDate = ShopHelper.DateParse(startDate);
        DateTime? validEndDate = ShopHelper.DateParse(endDate);

        int numberOfFutureDays = 5;

        if (validStartDate == null)
            validStartDate = DateTime.Now;

        if (validEndDate == null)
            validEndDate = DateTime.Now.AddDays(numberOfFutureDays);

        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var shopTotalWorkLoad = int.Parse(http.User.Claims.First(c => c.Type == "WorkLoad").Value);

        var schedules = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        schedules = schedules.Where(s => s.Date.Date >= validStartDate.Value.Date && s.Date.Date <= validEndDate.Value.Date).ToList();

        var schedulesIds = schedules.Select(s => s.Id).ToList();

        var demands = context.Demands.Include(d => d.Schedule).Include(d => d.Service).Where(d => schedulesIds.Contains(d.ScheduleId)).ToList();

        var schedulesDates = schedules.GroupBy(s => s.Date.Date);

        List<WorkLoadResponse> response = new List<WorkLoadResponse>();

        decimal constIncreaseInWorkload = (decimal)0.3;

        foreach (var date in schedulesDates)
        {
            var workLoadUsed = demands.Where(d => d.Schedule.Date.Date == date.Key).Sum(s => s.Service.WorkUnits);
            var workLoadAvailable = (shopTotalWorkLoad - workLoadUsed) >= 0 ? (shopTotalWorkLoad - workLoadUsed) : 0;
            bool hasIncreasedWorkLoad = date.Key.DayOfWeek == DayOfWeek.Thursday || date.Key.DayOfWeek == DayOfWeek.Friday;

            if (hasIncreasedWorkLoad)
                workLoadAvailable += (int)(shopTotalWorkLoad * constIncreaseInWorkload);

            var obj = new WorkLoadResponse(date.Key.ToString("dd/MM/yy"), workLoadAvailable, workLoadUsed);
            response.Add(obj);
        }

        response = response.OrderBy(r => r.Date).ToList();

        return Results.Ok(response);
    }
}
