using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Services;

public class ServicePut
{
    public static string Template => "/services/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ServiceRequest serviceRequest, HttpContext http, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;    
        var service = context.Services.FirstOrDefault(s => s.Id == id);

        if (service == null)
            return Results.NotFound("Service does not exist");

        service.EditInfo(serviceRequest.Name, serviceRequest.WorkUnits, userId);

        if (!service.IsValid)
            return Results.ValidationProblem(service.Notifications.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
