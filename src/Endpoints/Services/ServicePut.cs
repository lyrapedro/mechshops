using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Services;

public class ServicePut
{
    public static string Template => "/services/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ServiceRequest serviceRequest, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;    
        var service = context.Services.FirstOrDefault(s => s.Id == id && s.ShopId == shopId);

        var hasRelatedSchedule = context.Demands.Where(s => s.ServiceId == id).Any() && serviceRequest.WorkUnits > 0;

        if (hasRelatedSchedule)
            return Results.NotFound("You cannot delete this service because there are schedules related to it");

        if (service == null)
            return Results.NotFound("Service does not exist");

        service.EditInfo(serviceRequest.Name, serviceRequest.WorkUnits);

        if (!service.IsValid)
            return Results.ValidationProblem(service.Notifications.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
