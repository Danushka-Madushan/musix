using System.Text.Json;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace MusixApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        var wv = WebView.CoreWebView2;

        // ── Enable draggable regions (CSS -webkit-app-region: drag) ─────────
        wv.Settings.IsSwipeNavigationEnabled          = false;
        wv.Settings.AreDefaultContextMenusEnabled     = false;
        wv.Settings.IsNonClientRegionSupportEnabled   = true;   // FIX #1: activates CSS drag regions

        // ── Listen for messages from JavaScript ──────────────────────────────
        wv.WebMessageReceived += OnWebMessageReceived;

        // ── Navigate to the Kestrel-hosted page ──────────────────────────────
        wv.Navigate(((App)Application.Current).ServerUrl + "/index.html");
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            // Expected shape: { "action": "close" | "minimize" | "maximize" }
            // FIX #2: case-insensitive so JS "action" matches C# "Action"
            var msg = JsonSerializer.Deserialize<WindowMessage>(
                e.WebMessageAsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            switch (msg?.Action)
            {
                case "close":
                    Close();
                    break;
                case "minimize":
                    WindowState = WindowState.Minimized;
                    break;
                case "maximize":
                    WindowState = WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                    break;
            }
        }
        catch
        {
            // Ignore malformed messages
        }
    }

    private record WindowMessage(string Action);
}
