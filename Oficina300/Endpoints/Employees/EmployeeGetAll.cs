using Microsoft.AspNetCore.Authorization;
using Oficina300.Infra.Data;

namespace Oficina300.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
    {
        if (page == null || page.Value < 0)
            return Results.BadRequest("Page is required");

        if (rows == null || rows.Value < 0)
            rows = 10;

        var result = await query.Execute(page.Value, rows.Value);

        return Results.Ok(result);
    }
}
