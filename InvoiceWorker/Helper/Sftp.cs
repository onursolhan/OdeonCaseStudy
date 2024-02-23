using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;

namespace InvoiceWorker.Helper
{
    public static class Sftp
    {
        private static readonly string Host = "odeon_sftp_host";
        private static readonly int Port = 22;
        private static readonly string Username = "sftp_user";
        private static readonly string Password = "very_hard_password";
        private static readonly string InvoicesDirectory = "your_invoices_directory";

        public static List<string> GetFiles()
        {
            using (var client = new SftpClient(Host, Port, Username, Password))
            {
                try
                {
                    client.Connect();
                    return FindInvoiceFiles(client);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while fetching files: {ex.Message}", ex);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        public static void DownloadRemoteFile(string remoteFilePath, string localFilePath)
        {
            using (var client = new SftpClient(Host, Port, Username, Password))
            {
                try
                {
                    client.Connect();
                    using (var fileStream = File.Create(localFilePath))
                    {
                        client.DownloadFile(remoteFilePath, fileStream);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error downloading file: {ex.Message}");
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }

        private static List<string> FindInvoiceFiles(SftpClient client)
        {
            var files = client.ListDirectory(InvoicesDirectory);

            return files
                .Where(f => f.IsRegularFile && f.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.FullName)
                .ToList();
        }
    }
}
