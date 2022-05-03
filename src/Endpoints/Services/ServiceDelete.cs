using MechShops.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechShops.Endpoints.Services;

public class ServiceDelete
{
    public static string Template => "/services/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var service = context.Services.FirstOrDefault(s => s.ShopId == shopId && s.Id == id);

        var hasRelatedSchedule = context.Demands.Where(s => s.ServiceId == id).Any();

        if (hasRelatedSchedule)
            return Results.NotFound("You cannot delete this service because there are schedules related to it");

        if (service == null)
            return Results.NotFound("Service does not exist");

        context.Services.Remove(service);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
