using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Demands;

public class DemandDelete
{
    public static string Template => "/shops/demands/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;

        var demand = context.Demands.FirstOrDefault(d => d.Id == id);

        if (demand == null)
            return Results.NotFound("Demand does not exist");

        var demands = context.Demands.Where(s => s.Id == id).ToList();

        if (demands.Any())
            context.Demands.RemoveRange(demands);

        context.Demands.Remove(demand);

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
