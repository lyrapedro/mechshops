﻿using Microsoft.AspNetCore.Mvc;
using Oficina300.Domain.Shops;
using Oficina300.Endpoints.Shops;
using Oficina300.Infra.Data;
using System.Security.Claims;

namespace Oficina300.Endpoints.Services;

public class ServicePost
{
    public static string Template => "shops/{shopId:int}/services";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromRoute] int shopId, HttpContext http, ServiceRequest serviceRequest, ApplicationDbContext context)
    {
        var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var shop = context.Shops.FirstOrDefault(s => s.Id == shopId);

        if (shop == null)
            return Results.NotFound("Service does not exist");

        var service = new Service(serviceRequest.Name, serviceRequest.WorkUnits, shopId, userId);

        if (!service.IsValid)
            return Results.ValidationProblem(service.Notifications.ConvertToProblemDetails());

        await context.Services.AddAsync(service);

        await context.SaveChangesAsync();

        return Results.Created($"/services/{service.Id}", service.Id);
    }
}