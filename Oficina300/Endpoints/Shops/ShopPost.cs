using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Shops;

public class ShopPost
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ShopRequest shopRequest, HttpContext http, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var shop = new Shop(shopRequest.Name, shopRequest.WorkLoad);

        if (!shop.IsValid)
        {
            return Results.ValidationProblem(shop.Notifications.ConvertToProblemDetails());
        }

        await context.Shops.AddAsync(shop);
        await context.SaveChangesAsync();

        return Results.Created($"/shops/{shop.Id}", shop.Id);
    }
}
