using ByteMeBackup;
using Spectre.Console;
using Version = ByteMeBackup.Version;

AnsiConsole.Markup($"[bold white]Welcome to ByteMeBackup Scheduler v.{Version.VersionNumber}[/]");
AnsiConsole.Markup("[gray]Made by xyzjesper[/]\n");

AnsiConsole.Markup("[bold white]Starting ByteMeBackup Scheduler...[/]\n");

await Startup.Main();