using CommunityToolkit.Maui;
using TaskTracker.Maui.Features.Auth.ViewModels;
using TaskTracker.Maui.Features.Auth.Views;
using TaskTracker.Maui.Features.Documents.Popups;
using TaskTracker.Maui.Features.Documents.ViewModels;
using TaskTracker.Maui.Features.Projects.ViewModels;
using TaskTracker.Maui.Features.Projects.Views;
using TaskTracker.Maui.Features.Settings;
using TaskTracker.Maui.Features.Users.Popups;
using TaskTracker.Maui.Features.Users.ViewModels;
using TaskTracker.Maui.Features.Users.Views;
using TaskTracker.Maui.Features.WorkItems.ViewModels;
using TaskTracker.Maui.Features.WorkItems.Views;
using TaskTracker.Maui.Infrastructure;
using TaskTracker.Maui.Interfaces;
using TaskTracker.Maui.Popups;
using TaskTracker.Maui.Services;

namespace TaskTracker.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // ================= CONFIG =================
        // Синхронный, безопасный для раннего вызова метод —
        // не использует Essentials API, только Reflection и File IO.
        var appConfig = AppConfig.Load();
        var apiBaseUrl = appConfig.ApiBaseUrl;

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldEnableSnackbarOnWindows(true);
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // ================= HTTP CLIENT =================
        builder.Services.AddSingleton<TokenStore>();
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
        builder.Services.AddTransient<AuthHandler>();
        builder.Services.AddTransient<RetryHandler>();
        builder.Services.AddTransient(_ => new BaseAddressHandler(apiBaseUrl));

        builder.Services.AddHttpClient<AuthService>(c =>
        {
            c.BaseAddress = new Uri(apiBaseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1)
        })
        .AddHttpMessageHandler<BaseAddressHandler>()
        .AddHttpMessageHandler<RetryHandler>()
        .AddHttpMessageHandler<AuthHandler>();

        builder.Services.AddHttpClient<ApiService>(c =>
        {
            c.BaseAddress = new Uri(apiBaseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1)
        })
        .AddHttpMessageHandler<BaseAddressHandler>()
        .AddHttpMessageHandler<RetryHandler>()
        .AddHttpMessageHandler<AuthHandler>();

        // ================= VMs =================
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProjectsViewModel>();
        builder.Services.AddTransient<ProjectDetailsViewModel>();
        builder.Services.AddTransient<CreateProjectViewModel>();
        builder.Services.AddTransient<EditProjectViewModel>();
        builder.Services.AddTransient<WorkItemsViewModel>();
        builder.Services.AddTransient<CreateWorkItemViewModel>();
        builder.Services.AddTransient<DocumentsViewModel>();
        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<UserDetailsViewModel>();
        builder.Services.AddTransient<CreateUserViewModel>();
        builder.Services.AddTransient<EditUserViewModel>();

        // ================= VIEWS =================
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProjectsPage>();
        builder.Services.AddTransient<ProjectDetailsPage>();
        builder.Services.AddTransient<CreateProjectPage>();
        builder.Services.AddTransient<EditProjectPage>();
        builder.Services.AddTransient<CreateWorkItemPage>();
        builder.Services.AddTransient<EditWorkItemPopup>();
        builder.Services.AddTransient<DocumentsPopup>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<UserDetailsPage>();
        builder.Services.AddTransient<CreateUserPage>();
        builder.Services.AddTransient<EditUserPage>();
        builder.Services.AddTransient<UserPickerPopup>();
        builder.Services.AddTransient<SettingsPage>();

        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}