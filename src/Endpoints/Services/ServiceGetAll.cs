using Microsoft.AspNetCore.Mvc;
using MechShops.Endpoints.Shops;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Services;

public class ServiceGetAll
{
    public static string Template => "shops/services";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        var services = context.Services.Where(s => s.ShopId == shopId).ToList();
        var response = services.Select(s => new ServiceResponse(s.Id, s.Name, s.WorkUnits, s.ModifiedAt, s.CreatedAt));

        return Results.Ok(response);
    }
}
