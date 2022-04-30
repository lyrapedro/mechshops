using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Shops;

public class ShopDelete
{
    public static string Template => "/shops/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, ApplicationDbContext context)
    {
        var shop = context.Shops.Where(c => c.Id == id).FirstOrDefault();

        if (shop == null) return Results.NotFound("Shop does not exist");

        context.Remove(shop);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
