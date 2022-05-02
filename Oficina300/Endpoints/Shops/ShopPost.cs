using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Shops;

public class ShopPost
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ShopRequest shopRequest, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        var invalidCnpj = !ShopHelper.IsCnpj(shopRequest.Cnpj);
        if (invalidCnpj)
            return Results.BadRequest();

        var cnpjOnlyNumbers = ShopHelper.GetOnlyNumbers(shopRequest.Cnpj);

        var newShop = new IdentityUser { UserName = cnpjOnlyNumbers, Email = cnpjOnlyNumbers };
        var result = await userManager.CreateAsync(newShop, shopRequest.Password);

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        var userClaims = new List<Claim>
        {
            new Claim("ShopId", newShop.Id),
            new Claim("WorkLoad", shopRequest.WorkLoad.ToString())
        };

        await context.SaveChangesAsync();

        var defaultServices = new List<Service>()
        {
            new Service("Alinhamento de rodas", 1, newShop.Id),
            new Service("Lavação", 2, newShop.Id),
            new Service("Trocar óleo", 3, newShop.Id),
            new Service("Revisão básica", 5, newShop.Id),
            new Service("Revisão completa", 8, newShop.Id)
        };

        var claimResult = await userManager.AddClaimsAsync(newShop, userClaims);

        if (!claimResult.Succeeded)
            return Results.ValidationProblem(claimResult.Errors.ConvertToProblemDetails());

        await context.Services.AddRangeAsync(defaultServices);
        await context.SaveChangesAsync();

        return Results.Created($"/shops/{newShop.Id}", newShop.Id);
    }
}
