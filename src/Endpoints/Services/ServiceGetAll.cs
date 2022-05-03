using MechShops.Infra.Data;
using Microsoft.AspNetCore.Authorization;

namespace MechShops.Endpoints.Services;

public class ServiceGetAll
{
    public static string Template => "/services";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static IResult Action(HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var services = context.Services.Where(s => s.ShopId == shopId).ToList();
        var response = services.Select(s => new ServiceResponse(s.Id, s.Name, s.WorkUnits, s.ModifiedAt, s.CreatedAt));

        return Results.Ok(response);
    }
}
