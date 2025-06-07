using CryptoCore.BackgroundServices;
using CryptoCore.Services.Implements;
using CryptoCore.Services.Interfaces;
using CryptoInfrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceBusDelivery;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SQLServerCryptoConnection") ?? throw new InvalidOperationException("Connection string 'SQLServerIdentityConnection' not found.");
var sbConn = builder.Configuration.GetConnectionString("ServiceBusConnection");
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtAccessToken:Key"] ?? "");
var allowedHosts = builder.Configuration["AllowedHosts"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<CryptoDbcontext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
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

builder.Services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
    x => new ServiceBusQueue(sbConn ?? ""));

builder.Services.AddHostedService<AzureServiceBusListener>();
builder.Services.AddScoped<ICryptoServices, CryptoServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IGroupServices, GroupServices>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
