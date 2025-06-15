using System.Text.Json;
using ByteMeBackup.Configuration;
using Spectre.Console;

namespace ByteMeBackup.Services;

public class ConfigService
{
    public ConfigService(AppConfig? config)
    {
        Config = config;
    }

    private AppConfig? Config { get; set; }

    public async Task CreateConfig()
    {
        var configFile = File.Exists("config.json")
            ? "config.json"
            : Path.Combine(AppContext.BaseDirectory, "config.json");
        if (!File.Exists(configFile))
        {
            AnsiConsole.Markup("[red]Configuration file not found![/]\n");

            var defaultConfig = JsonSerializer.Serialize(new AppConfig
            {
                BackupConfigs =
                [
                    new BackupConfig
                    {
                        BackupPath = "",
                        BackupPrefix = "",
                        // BackupSchedule = "",
                        BackupType = BackupType.MountedDrive,
                        MountedDrivePath = ""
                    }
                ],
                DiscordWebhookUrl = "",
                RemoteServer =
                {
                    IpAddress = "",
                    Password = "",
                    Port = "",
                    Username = ""
                }
            }, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            });
            await File.WriteAllTextAsync("config.json", defaultConfig);
            AnsiConsole.Markup("[green]Default configuration file created![/]\n");
        }

        AnsiConsole.Markup("[bold white]Loading configuration...[/]\n");
        var configContent = await File.ReadAllTextAsync("config.json");
        Config = JsonSerializer.Deserialize<AppConfig>(configContent);
        AnsiConsole.Markup("[green]Configuration loaded successfully![/]\n");
    }

    public AppConfig? Get() => Config;
}