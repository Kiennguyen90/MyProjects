using CryptoInfrastructure;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserCore.Services.Implements;
using UserCore.Services.Interfaces;
using UserManagementAPI.Middlewares;
using UserCore;
using Microsoft.AspNetCore.Authentication.Google;
using ServiceBusDelivery;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SQLServerIdentityConnection") ?? throw new InvalidOperationException("Connection string 'SQLServerIdentityConnection' not found.");
var sbConn = builder.Configuration.GetConnectionString("ServiceBusConnection");
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtAccessToken:Key"] ??"");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
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

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
    x => new ServiceBusQueue(sbConn ?? ""));
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IAplicationServices, ApplicationServices>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var allowedHosts = builder.Configuration["AllowedHosts"];

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
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
