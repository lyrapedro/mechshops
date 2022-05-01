using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Services;

public class ServiceDelete
{
    public static string Template => "/services/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ApplicationDbContext context)
    {
        var service = context.Services.FirstOrDefault(s => s.Id == id);

        if (service == null)
            return Results.NotFound("Service does not exist");

        var services = context.Services.Where(s => s.ShopId == service.Id).ToList();

        if (services.Any())
            context.Services.RemoveRange(services);

        context.Services.Remove(service);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
