using System.Threading.Tasks;
using ByteMeBackup.Configuration;
using ByteMeBackup.Services;
using Spectre.Console;

namespace ByteMeBackup;

public class Startup
{
    private static ConfigService ConfigService;
    private static AppConfig Config;

    public static async Task Main()
    {
        var configService = new ConfigService();
        await configService.CreateConfig();
        Config = ConfigService.Get();

        AnsiConsole.Markup("[bold white]Starting ByteMeBackup Scheduler...[/]\n");

        if (Config.BackupConfigs.Length == 0)
        {
            AnsiConsole.Markup("[red]No backup configurations found! Please check your config.json file.[/]\n");
            return;
        }

        foreach (var backupConfig in Config.BackupConfigs)
        {
            var backupTask = new Core.BackupTask(backupConfig, Config.RemoteServer);
            AnsiConsole.Markup($"[bold white]Running backup for: {backupConfig.BackupPath}[/]\n");
            backupTask.Run();
        }

        AnsiConsole.Markup("[green]All backups completed![/]\n");
    }
}