using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Shops;

public class ShopPut
{
    public static string Template => "/shops/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ShopRequest shopRequest, HttpContext http, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;    
        var shop = context.Shops.Where(c => c.Id == id).FirstOrDefault();

        if (shop == null) return Results.NotFound("Shop does not exist");

        shop.EditInfo(shopRequest.Name, shopRequest.WorkLoad, userId);

        if (!shop.IsValid) return Results.ValidationProblem(shop.Notifications.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
