using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;
using TaskTracker.Infrastructure.Persistence;
using TaskTracker.Infrastructure.Repositories;
using TaskTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// DbContext

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


// Identity

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// JWT Authentication

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });


// HttpContext

builder.Services.AddHttpContextAccessor();


// Repositories

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();


// Services

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();


var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();