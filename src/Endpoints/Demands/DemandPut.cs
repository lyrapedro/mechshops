using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MechShops.Infra.Data;
using System.Security.Claims;

namespace MechShops.Endpoints.Demands;

public class DemandPut
{
    public static string Template => "/demands/{id:int}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] int id, DemandRequest demandRequest, HttpContext http, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;    
        var demand = context.Demands.FirstOrDefault(s => s.Id == id);

        if (demand == null)
            return Results.NotFound("Demand does not exist");

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
