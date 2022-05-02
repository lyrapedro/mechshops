using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MechShops.Endpoints.Schedules;
using MechShops.Endpoints.Security;
using MechShops.Endpoints.Services;
using MechShops.Endpoints.Shops;
using MechShops.Infra.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionStrings:DefaultConnection"],
                sinkOptions: new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = "Logs"
                });
});
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:DefaultConnection"]);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("ShopPolicy", p => p.RequireAuthenticatedUser().RequireClaim("ShopId"));
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.MapMethods(ShopPost.Template, ShopPost.Methods, ShopPost.Handle);
app.MapMethods(ShopPut.Template, ShopPut.Methods, ShopPut.Handle);
app.MapMethods(ShopDelete.Template, ShopDelete.Methods, ShopDelete.Handle);

app.MapMethods(GetWorkLoadByDayOrInterval.Template, GetWorkLoadByDayOrInterval.Methods, GetWorkLoadByDayOrInterval.Handle);

app.MapMethods(SchedulePost.Template, SchedulePost.Methods, SchedulePost.Handle);
app.MapMethods(ScheduleGetAll.Template, ScheduleGetAll.Methods, ScheduleGetAll.Handle);
app.MapMethods(ScheduleDelete.Template, ScheduleDelete.Methods, ScheduleDelete.Handle);

app.MapMethods(ServicePost.Template, ServicePost.Methods, ServicePost.Handle);
app.MapMethods(ServiceGetAll.Template, ServiceGetAll.Methods, ServiceGetAll.Handle);
app.MapMethods(ServicePut.Template, ServicePut.Methods, ServicePut.Handle);
app.MapMethods(ServiceDelete.Template, ServiceDelete.Methods, ServiceDelete.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
    }

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();