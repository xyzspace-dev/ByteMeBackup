using System;
using System.IO;
using System.IO.Compression;
using ByteMeBackup.Configuration;
using Renci.SshNet;

namespace ByteMeBackup.Core;

public class BackupTask
{
    private readonly BackupConfig BackupConfig;
    private readonly RemoteServerConfig RemoteServerConfig;

    public BackupTask(BackupConfig backupConfig, RemoteServerConfig remoteServerConfig)
    {
        BackupConfig = backupConfig;
        RemoteServerConfig = remoteServerConfig;
    }

    public void Run()
    {
        try
        {
            Console.WriteLine($"[BackupTask] Starting backup for: {BackupConfig.BackupPath}");

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var zipFileName = $"{BackupConfig.BackupPrefix}{timestamp}.zip";
            var tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);

            // Create the backup zip file
            ZipFile.CreateFromDirectory(BackupConfig.BackupPath, tempZipPath);
            Console.WriteLine($"[BackupTask] Backup created: {tempZipPath}");

            switch (BackupConfig.BackupType)
            {
                case BackupType.MountedDrive:
                    HandleMountedDriveBackup(tempZipPath, zipFileName);
                    break;

                case BackupType.ToServer:
                    HandleSftpBackup(tempZipPath, zipFileName);
                    break;

                default:
                    Console.WriteLine($"[BackupTask] Unknown backup type: {BackupConfig.BackupType}");
                    break;
            }

            File.Delete(tempZipPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BackupTask] Error: {ex.Message}");
        }
    }

    private void HandleMountedDriveBackup(string sourceZipPath, string fileName)
    {
        var targetPath = Path.Combine(BackupConfig.MountedDrivePath, fileName);
        Directory.CreateDirectory(BackupConfig.MountedDrivePath);
        File.Copy(sourceZipPath, targetPath, overwrite: true);
        Console.WriteLine($"[BackupTask] Backup saved to mounted drive: {targetPath}");
    }

    private void HandleSftpBackup(string sourceZipPath, string fileName)
    {
        using var client =
            new SftpClient(RemoteServerConfig.IpAddress, int.Parse(RemoteServerConfig.Port),
                RemoteServerConfig.Username, RemoteServerConfig.Password);
        client.Connect();
        using var fileStream = File.OpenRead(sourceZipPath);
        client.UploadFile(fileStream, fileName);
        client.Disconnect();
        Console.WriteLine($"[BackupTask] Backup uploaded to server as: {fileName}");
    }
}