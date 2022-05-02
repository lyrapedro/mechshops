using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Services;

public class ServiceDelete
{
    public static string Template => "shops/services/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action([FromRoute] int id, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var service = context.Services.FirstOrDefault(s => s.ShopId == shopId && s.Id == id);

        if (service == null)
            return Results.NotFound("Service does not exist");

        context.Services.Remove(service);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
