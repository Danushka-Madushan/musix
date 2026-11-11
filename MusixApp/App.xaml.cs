using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace MusixApp;

public partial class App : Application
{
    public const int Port = 5173;
    public string ServerUrl => $"http://localhost:{Port}";

    private WebApplication? _webApp;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ── Build & start Kestrel ────────────────────────────────────────────
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(ServerUrl);
        // Suppress the default ASP.NET Core console banner
        builder.Logging.ClearProviders();

        _webApp = builder.Build();

        // Serve static files from wwwroot (resolved relative to the executable)
        var wwwroot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        _webApp.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(wwwroot),
            RequestPath  = ""
        });

        // Sample JSON API endpoint
        _webApp.MapGet("/api/ping", () => new { status = "ok", port = Port });

        await _webApp.StartAsync();

        // ── Open the main window after the server is ready ───────────────────
        new MainWindow().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_webApp is not null)
        {
            await _webApp.StopAsync();
            await _webApp.DisposeAsync();
        }
        base.OnExit(e);
    }
}
