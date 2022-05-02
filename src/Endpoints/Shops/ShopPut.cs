using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Shops;

public class ShopPut
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static async Task<IResult> Action(ShopRequestPut shopRequestPut, UserManager<IdentityUser> userManager, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var workLoadClaim = http.User.Claims.First(c => c.Type == "WorkLoad");

        IdentityUser shop = await userManager.FindByIdAsync(shopId);

        if (shop == null)
            return Results.NotFound("Shop does not exist");

        await userManager.RemoveClaimAsync(shop, workLoadClaim);
        var claimResult = await userManager.AddClaimAsync(shop, new Claim("WorkLoad", shopRequestPut.WorkLoad.ToString()));

        var result = await userManager.UpdateAsync(shop);

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
