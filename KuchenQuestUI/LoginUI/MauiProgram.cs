using LoginUI.Components.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using System.Net.Http;

namespace LoginUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("ComicSansMS.ttf", "ComicSansMS"); // Unverändert
            })
            .Services.AddScoped<UserService>()
            .AddScoped<APIService>();

        builder.Services.AddMauiBlazorWebView();

        // HttpClient für API-Anfragen registrieren
        builder.Services.AddScoped(sp =>
        {
            var handler = new HttpClientHandler();

            // Achtung: Deaktiviert SSL-Zertifikatsprüfung (nur für Entwicklung!)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            return new HttpClient(handler)
            {
                BaseAddress = new Uri($"https://{APIService.API_IP}:7067/KuechenQuest/") // Die URL der API
            };
        });

        // **Hier wird der Datenbankpfad definiert:**
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "KuechenQuest.db");

        // **Hier wird der DbContext registriert:**
        //builder.Services.AddDbContext<KuechenQuestDbContext>(options =>
        //    options.UseSqlite($"Data Source={dbPath}")
        //);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
