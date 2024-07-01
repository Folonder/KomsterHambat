using KomsterHambatAutoClicker.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KomsterHambatAutoClicker;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var autoClicker = host.Services.GetRequiredService<AutoClicker>();

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        await autoClicker.RunAsync(cts.Token);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<AutoClicker>();
                services.AddSingleton<IKomsterHambatClient, KomsterHambatClient>();
                services.AddSingleton<IStateManager, StateManager>();
                services.Configure<KomsterHambatClientOptions>(hostContext.Configuration.GetSection("KomsterHambatClientOptions"));
                services.Configure<StateManagerOptions>(hostContext.Configuration.GetSection("StateManagerOptions"));
            });
}