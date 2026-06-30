using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not set");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

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
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// ================= SERVICES =================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

// ================= USE CASES =================
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IApplicationMarker>()
    .AddClasses(classes => classes.Where(type =>
        type.Name.EndsWith("UseCase")))
    .AsSelf()
    .WithScopedLifetime());

// ================= ADMIN SEED OPTIONS =================
builder.Services
    .AddOptions<AdminUserOptions>()
    .Bind(builder.Configuration.GetSection(AdminUserOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Email), "Admin:Email is not set")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "Admin:Password is not set")
    .ValidateOnStart();

// ================= SEEDER =================
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<IdentitySeederHostedService>();
}

var app = builder.Build();

// ================= MIDDLEWARE ORDER =================
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// ================= OPENAPI =================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ================= CONTROLLERS =================
app.MapControllers();

// ================= MIGRATIONS =================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

app.Run();