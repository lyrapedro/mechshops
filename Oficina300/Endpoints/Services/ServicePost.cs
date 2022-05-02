using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Services;

public class ServicePost
{
    public static string Template => "shops/services";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action(HttpContext http, ServiceRequest serviceRequest, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var service = new Service(serviceRequest.Name, serviceRequest.WorkUnits, shopId);

        if (!service.IsValid)
            return Results.ValidationProblem(service.Notifications.ConvertToProblemDetails());

        await context.Services.AddAsync(service);

        await context.SaveChangesAsync();

        return Results.Created($"/services/{service.Id}", service.Id);
    }
}
