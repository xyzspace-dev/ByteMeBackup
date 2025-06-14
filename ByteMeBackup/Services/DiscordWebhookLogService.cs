using System.Net.Http.Json;
using Spectre.Console;

namespace ByteMeBackup.Services;

public class DiscordWebhookLogService
{
    private ConfigService ConfigService;

    public DiscordWebhookLogService(ConfigService configService)
    {
        ConfigService = configService;
    }

    public async Task SendLogAsync(string message)
    {
        var config = ConfigService.Get();
        if (string.IsNullOrEmpty(config.DiscordWebhookUrl))
        {
            AnsiConsole.Markup("[red]Discord webhook URL is not configured![/]\n");
            return;
        }

        using var httpClient = new HttpClient();
        var payload = new
        {
            content = message,
            username = "ByteMeBackup",
            avatar_url = "https://i.imgur.com/ytvbK23.png"
        };

        var response = await httpClient.PostAsJsonAsync(config.DiscordWebhookUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to send log to Discord: {response.ReasonPhrase}");
        }
    }
}