namespace ByteMeBackup.Helper;

using System.IO;

public class FileHelper
{
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            using var sourceFileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite);
            using var targetFileStream = new FileStream(targetFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
            sourceFileStream.CopyTo(targetFileStream);
            sourceFileStream.Close();
            targetFileStream.Close();
        }

        // If recursive and copying subdirectories, recursively call this method
        if (!recursive) return;
        foreach (var subDir in dirs)
        {
            string newDestinationDir;
            newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }
}