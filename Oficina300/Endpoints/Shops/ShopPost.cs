using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Shops;

public class ShopPost
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ShopRequest shopRequest, HttpContext http, ApplicationDbContext context)
    {
        var shop = new Shop(shopRequest.Name, shopRequest.WorkLoad);

        if (!shop.IsValid)
            return Results.ValidationProblem(shop.Notifications.ConvertToProblemDetails());

        await context.Shops.AddAsync(shop);
        await context.SaveChangesAsync();

        string guidString = Guid.Empty.ToString();

        var defaultServices = new List<Service>()
        {
            new Service("Alinhamento de rodas", 1, shop.Id, guidString),
            new Service("Lavação", 2, shop.Id, guidString),
            new Service("Trocar óleo", 3, shop.Id, guidString),
            new Service("Revisão básica", 5, shop.Id, guidString),
            new Service("Revisão completa", 8, shop.Id, guidString)
        };
        await context.Services.AddRangeAsync(defaultServices);

        await context.SaveChangesAsync();

        return Results.Created($"/shops/{shop.Id}", shop.Id);
    }
}
