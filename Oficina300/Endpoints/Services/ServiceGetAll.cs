using Microsoft.AspNetCore.Mvc;
using Oficina300.Endpoints.Shops;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Services;

public class ServiceGetAll
{
    public static string Template => "shops/{shopId:int}/services";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] int shopId, ApplicationDbContext context)
    {
        var services = context.Services.Where(s => s.ShopId == shopId).ToList();
        var response = services.Select(s => new ServiceResponse(s.Id, s.Name, s.WorkUnits, s.ModifiedAt, s.CreatedAt));

        return Results.Ok(response);
    }
}
