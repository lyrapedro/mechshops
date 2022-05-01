using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Domain.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Demands;

public class DemandPost
{
    public static string Template => "shops/{shopId:int}/demands";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int shopId, HttpContext http, DemandRequest demandRequest, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var shop = context.Shops.FirstOrDefault(s => s.Id == shopId);

        if (shop == null)
            return Results.NotFound("Demand does not exist");

        var demand = new Demand(demandRequest.Date, userId, shopId);

        await context.Demands.AddAsync(demand);

        await context.SaveChangesAsync();

        return Results.Created($"/demands/{demand.Id}", demand.Id);
    }
}
