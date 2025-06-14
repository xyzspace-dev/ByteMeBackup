using ByteMeBackup.Configuration;
using Spectre.Console;

namespace ByteMeBackup.Helper;

public class CommandHelper
{
    public static void RunCommand(string command)
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(processInfo);
        if (process == null) return;

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        AnsiConsole.Markup(!string.IsNullOrEmpty(output)
            ? $"[blue]Command output:[/]\n{output}[/]\n"
            : "[blue]No output from command.[/]\n");

        if (!string.IsNullOrEmpty(error))
        {
            AnsiConsole.Markup($"[red]Error executing command: {error}[/]\n");
            return;
        }

        AnsiConsole.Markup("[green]Command executed successfully![/]\n");
    }

    public static async Task<string> RunCommandAsync(string command)
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(processInfo);
        if (process == null) return string.Empty;
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        AnsiConsole.Markup(!string.IsNullOrEmpty(output)
            ? $"[blue]Command output:[/]\n{output}[/]\n"
            : "[blue]No output from command.[/]\n");
        if (!string.IsNullOrEmpty(error))
        {
            AnsiConsole.Markup($"[red]Error executing command: {error}[/]\n");
            return string.Empty;
        }

        AnsiConsole.Markup("[green]Command executed successfully![/]\n");
        return output;
    }
}