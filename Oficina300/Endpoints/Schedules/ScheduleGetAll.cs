using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Schedules;

public class ScheduleGetAll
{
    public static string Template => "shops/{shopId:int}/services";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] int shopId, ApplicationDbContext context)
    {
        var services = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        var response = services.Select(s => new ScheduleResponse(s.Id, s.ShopId, s.CreatedBy, s.ModifiedAt, s.CreatedAt));

        return Results.Ok(response);
    }
}
