using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MechShops.Infra.Data;

namespace MechShops.Endpoints.Shops;

public class ShopDelete
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action(UserManager<IdentityUser> userManager, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;
        IdentityUser shop = await userManager.FindByIdAsync(shopId);

        if (shop == null)
            return Results.NotFound("Shop does not exist");

        var result = await userManager.DeleteAsync(shop);

        var schedules = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        var services = context.Services.Where(s => s.ShopId == shopId).ToList();

        if (schedules.Any())
            context.Schedules.RemoveRange(schedules);

        if (services.Any())
            context.Services.RemoveRange(services);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
