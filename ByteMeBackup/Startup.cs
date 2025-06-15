using System.Threading.Tasks;
using ByteMeBackup.Configuration;
using ByteMeBackup.Services;
using Spectre.Console;

namespace ByteMeBackup;

public class Startup
{
    private static ConfigService ConfigService;
    private static AppConfig? Config;

    public static async Task Main()
    {
        var configService = new ConfigService(new AppConfig());
        await configService.CreateConfig();
        Config = configService.Get();

        AnsiConsole.Markup("[bold white]Starting ByteMeBackup...[/]\n");

        if (Config.BackupConfigs.Length == 0)
        {
            AnsiConsole.Markup("[red]No backup configurations found! Please check your config.json file.[/]\n");
            return;
        }

        foreach (var backupConfig in Config.BackupConfigs)
        {
            var backupTask = new Core.BackupTask(backupConfig, Config.RemoteServer,
                new DiscordWebhookLogService(configService));
            AnsiConsole.Markup($"[bold white]Running backup for: {backupConfig.BackupPath}[/]\n");
            await backupTask.Run();
        }

        AnsiConsole.Markup("[yellow bold]All backups completed![/]\n");
    }
}