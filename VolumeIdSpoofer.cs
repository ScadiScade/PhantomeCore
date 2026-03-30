using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhantomCore
{
    public class VolumeIdSpoofer
    {
        private const string VolumeIdUrl = "https://live.sysinternals.com/Volumeid64.exe";
        private const string LocalToolPath = "Volumeid64.exe";

        public static void SpoofVolumeId()
        {
            Console.WriteLine("  [*] Changing Volume ID (Volume Serial Number)...");

            try
            {
                if (!File.Exists(LocalToolPath))
                {
                    Console.WriteLine("  [*] Downloading Sysinternals VolumeId tool...");
                    DownloadTool().Wait();
                }

                if (!File.Exists(LocalToolPath))
                {
                    Console.WriteLine("  [-] Error: VolumeId tool not found and could not be downloaded.");
                    return;
                }

                // Accept EULA for Sysinternals
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Sysinternals\VolumeId", "EulaAccepted", 1);

                // Get list of all logical drives (C:\, D:\ etc.)
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                    {
                        string driveLetter = drive.Name.Substring(0, 2); // e.g., "C:"
                        string newVolId = GenerateRandomVolumeId();
                        
                        Console.WriteLine($"  [*] Applying new Volume ID ({newVolId}) for drive {driveLetter}...");
                        
                        bool success = ExecuteVolumeId(driveLetter, newVolId);
                        if (success)
                        {
                            Console.WriteLine($"  [+] Volume ID for {driveLetter} successfully changed.");
                        }
                    }
                }
                
                Console.WriteLine("  [!] PC restart is required to apply new Volume IDs.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Error changing Volume ID: {ex.Message}");
            }
        }

        private static async Task DownloadTool()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(VolumeIdUrl);
                    File.WriteAllBytes(LocalToolPath, fileBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Ошибка скачивания: {ex.Message}");
            }
        }

        private static string GenerateRandomVolumeId()
        {
            Random rnd = new Random();
            // Format is XXXX-XXXX (Hex)
            return $"{rnd.Next(0x1000, 0xFFFF):X4}-{rnd.Next(0x1000, 0xFFFF):X4}";
        }

        private static bool ExecuteVolumeId(string driveLetter, string newId)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = LocalToolPath;
                process.StartInfo.Arguments = $"{driveLetter} {newId}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}