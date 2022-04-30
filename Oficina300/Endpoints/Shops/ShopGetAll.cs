using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Shops;

public class ShopGetAll
{
    public static string Template => "/shops";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(ApplicationDbContext context)
    {
        var shops = context.Shops.ToList();
        var response = shops.Select(c => new ShopResponse(c.Id, c.Name, c.WorkLoad, c.ModifiedAt, c.CreatedAt));

        return Results.Ok(response);
    }
}
