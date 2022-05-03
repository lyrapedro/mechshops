using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechShops.Domain.Shops;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Services;

public class ServicePost
{
    public static string Template => "shops/services";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action(HttpContext http, ServiceRequest serviceRequest, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;

        var service = new Service(serviceRequest.Name, serviceRequest.WorkUnits, shopId);

        if (!service.IsValid)
            return Results.ValidationProblem(service.Notifications.ConvertToProblemDetails());

        await context.Services.AddAsync(service);

        await context.SaveChangesAsync();

        return Results.Created($"/services/{service.Id}", service.Id);
    }
}
