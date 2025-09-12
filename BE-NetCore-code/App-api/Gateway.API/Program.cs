using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtAccessToken:Key"] ?? "");
var allowedHosts = builder.Configuration["AllowedHosts"];
builder.Services.AddOpenApi();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHealthChecks();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JwtAccessToken:Issuer"],
        ValidAudience = builder.Configuration["JwtAccessToken:Audience"],
        RequireExpirationTime = true,
    };
});

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("application-access",
        p => p.RequireAuthenticatedUser().RequireClaim("application-access", true.ToString()));
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy (
        op =>
        {
            if (string.IsNullOrEmpty(allowedHosts) || allowedHosts == "*")
            {
                op.AllowAnyOrigin();
            }
            else
            {
                op.WithOrigins(allowedHosts.Split(';'));
            }
            op.AllowAnyHeader();
            op.AllowAnyMethod();
        });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("test",() => Results.Ok("test succes")).RequireAuthorization("application-access");
app.UseHttpsRedirection();
app.MapReverseProxy();
app.MapHealthChecks("health");

app.Run();