using Spectre.Console;

namespace ByteMeBackup.Helper;

using System.IO;

public class FileHelper
{
    public static void CopyDirectory(string sourceDir, string destinationDir)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        AnsiConsole.Status()
            .Start("Start process", ctx =>
            {
                // Update the status and spinner
                ctx.Status("Copy files...");
                ctx.Spinner(Spinner.Known.Dots2);
                ctx.SpinnerStyle(Style.Parse("gray"));

                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        var targetFilePath = Path.Combine(destinationDir, file.Name);
                        using var sourceFileStream = new FileStream(file.FullName, FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.ReadWrite);
                        using var targetFileStream =
                            new FileStream(targetFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
                        sourceFileStream.CopyTo(targetFileStream);
                        
                        ctx.Status($"Copy file {file.Name} to {targetFilePath}");
                        
                        sourceFileStream.Close();
                        targetFileStream.Close();
                    }
                    catch (IOException e)
                    {
                        AnsiConsole.Markup($"[gray]Skip file because of IOException by {file.FullName}[/]");
                    }
                }
            });
    }
}