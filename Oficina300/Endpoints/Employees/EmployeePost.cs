﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Oficina300.Endpoints.Employees;

public class EmployeePost
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http, UserManager<IdentityUser> userManager)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var newUser = new IdentityUser { UserName = employeeRequest.Cpf, Email = employeeRequest.Cpf };
        var result = await userManager.CreateAsync(newUser, employeeRequest.Password);

        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", employeeRequest.EmployeeCode),
            new Claim("Name", employeeRequest.Name),
            new Claim("CreatedBy", userId),
        };

        var claimResult = await userManager.AddClaimsAsync(newUser, userClaims);

        if (!claimResult.Succeeded)
            return Results.ValidationProblem(claimResult.Errors.ConvertToProblemDetails());

        return Results.Created($"/employees/{newUser.Id}", newUser.Id);
    }
}