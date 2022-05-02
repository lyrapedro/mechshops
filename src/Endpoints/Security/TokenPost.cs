﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MechShops.Endpoints.Shops;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MechShops.Endpoints.Security;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(LoginRequest loginRequest, IConfiguration configuration, UserManager<IdentityUser> userManager, ILogger<TokenPost> log)
    {
        log.LogInformation("Getting token");

        var invalidCnpj = !ShopHelper.IsCnpj(loginRequest.Cnpj);
        if (invalidCnpj)
            return Results.BadRequest();

        var cnpjOnlyNumbers = ShopHelper.GetOnlyNumbers(loginRequest.Cnpj);

        var user = await userManager.FindByEmailAsync(cnpjOnlyNumbers);
        if (user == null)
            Results.BadRequest();
        if (!await userManager.CheckPasswordAsync(user, loginRequest.Password))
            Results.BadRequest();

        var claims = await userManager.GetClaimsAsync(user);
        var subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, cnpjOnlyNumbers),
            new Claim("ShopId", user.Id),
        });
        subject.AddClaims(claims);

        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSettings:Audience"],
            Issuer = configuration["JwtBearerTokenSettings:Issuer"],
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new
        {
            token = tokenHandler.WriteToken(token)
        });
    }
}
