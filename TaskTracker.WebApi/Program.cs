using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using TaskTracker.Api.Filters;
using TaskTracker.Api.Middleware;
using TaskTracker.Application;
using TaskTracker.Application.Interfaces;
using TaskTracker.Infrastructure.Identity;
using TaskTracker.Infrastructure.Persistence;
using TaskTracker.Infrastructure.Repositories;
using TaskTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ================= CONTROLLERS =================

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// ================= VALIDATORS =================

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ================= OPENAPI =================

builder.Services.AddOpenApi();


// ================= SCALAR UI =================

builder.Services.AddEndpointsApiExplorer();

// ================= DATABASE =================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


// ================= IDENTITY =================

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ================= JWT =================

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

// ================= HTTP CONTEXT =================

builder.Services.AddHttpContextAccessor();


// ================= REPOSITORIES =================

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();


// ================= SERVICES =================

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();


// ================= USE CASES (SCRUTOR) =================

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IApplicationMarker>()
    .AddClasses(classes => classes.Where(type =>
        type.Name.EndsWith("UseCase")))
    .AsSelf()
    .WithScopedLifetime());


// ================= APP =================

var app = builder.Build();


// ================= OPENAPI =================

app.MapOpenApi();


// ================= SCALAR =================

app.MapScalarApiReference();


// ================= AUTH =================

app.UseAuthentication();

app.UseAuthorization();

// ================= MIDDLEWARE =================

app.UseMiddleware<ExceptionMiddleware>();

// ================= CONTROLLERS =================

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedAsync(roleManager);

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<AppUser>>();

    await AdminSeeder.SeedAsync(userManager);
}


app.Run();