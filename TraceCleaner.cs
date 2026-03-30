using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace PhantomCore
{
    public class TraceCleaner
    {
        public static void DeepClean()
        {
            Console.WriteLine("\n  [*] Starting deep trace cleaning...");
            
            CleanTempFolders();
            CleanBrowserCaches();
            CleanSystemLogs();
            CleanRegistryTraces();
            CleanAdvertisingAndBAM();
            
            Console.WriteLine("  [+] Deep cleaning completed.\n");
        }

        private static void CleanTempFolders()
        {
            Console.WriteLine("  [*] Cleaning temporary folders...");
            
            string[] tempPaths = {
                Path.GetTempPath(),
                @"C:\Windows\Temp",
                @"C:\Windows\Prefetch",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps")
            };

            foreach (string path in tempPaths)
            {
                if (Directory.Exists(path))
                {
                    DeleteDirectoryContents(path);
                }
            }
        }

        private static void CleanBrowserCaches()
        {
            Console.WriteLine("  [*] Очистка кэша браузеров и приложений...");
            
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Пример путей (Trae, Chrome, Edge, Discord и т.д.)
            string[] appPaths = {
                Path.Combine(roamingAppData, "Trae", "Cache"),
                Path.Combine(roamingAppData, "Trae", "Code Cache"),
                Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Cache"),
                Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Cache")
            };

            foreach (string path in appPaths)
            {
                if (Directory.Exists(path))
                {
                    DeleteDirectoryContents(path);
                }
            }
        }

        private static void CleanSystemLogs()
        {
            Console.WriteLine("  [*] Очистка системных журналов (Event Viewer)...");
            
            try
            {
                // Для очистки журналов требуются права Администратора
                Process process = new Process();
                process.StartInfo.FileName = "wevtutil.exe";
                process.StartInfo.Arguments = "el"; // Перечислить логи
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string[] logs = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Очищаем только основные, чтобы не тратить много времени, 
                // или можно пройтись по всему массиву logs.
                string[] targetLogs = { "Application", "Security", "Setup", "System" };
                
                foreach (string log in targetLogs)
                {
                    Process clrProc = new Process();
                    clrProc.StartInfo.FileName = "wevtutil.exe";
                    clrProc.StartInfo.Arguments = $"cl \"{log}\"";
                    clrProc.StartInfo.UseShellExecute = false;
                    clrProc.StartInfo.CreateNoWindow = true;
                    clrProc.Start();
                    clrProc.WaitForExit();
                }
                Console.WriteLine("  [+] Основные системные журналы очищены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Ошибка при очистке журналов: {ex.Message}");
            }
        }

        private static void CleanRegistryTraces()
        {
            Console.WriteLine("  [*] Cleaning registry traces (USB History and RunMRU)...");
            
            // Remove USB history (often used for identification)
            try
            {
#pragma warning disable CA1416
                // Clearing device list in registry (requires SYSTEM privileges for full removal, 
                // but we can delete some accessible keys or use PsExec in a real app)
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USBSTOR", true))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            try { key.DeleteSubKeyTree(subKeyName, false); } catch { /* Ignore access errors */ }
                        }
                    }
                }
                
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB", true))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            // Delete only VID/PID keys, avoid ROOT hubs
                            if (subKeyName.StartsWith("VID_")) 
                            {
                                try { key.DeleteSubKeyTree(subKeyName, false); } catch { }
                            }
                        }
                    }
                }
#pragma warning restore CA1416
            }
            catch { }
            
            // Clear recently run programs history (RunMRU)
            try
            {
#pragma warning disable CA1416
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", true))
                {
                    if (key != null)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            key.DeleteValue(valueName, false);
                        }
                    }
                }
#pragma warning restore CA1416
            }
            catch { }
            
            Console.WriteLine("  [+] Registry traces cleared (as much as Administrator privileges allow).");
        }

        private static void CleanAdvertisingAndBAM()
        {
            Console.WriteLine("  [*] Очистка телеметрии (Advertising ID, BAM, SRUM)...");
            
            try
            {
#pragma warning disable CA1416
                // Очистка Advertising ID
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", true))
                {
                    if (key != null)
                    {
                        key.SetValue("Id", Guid.NewGuid().ToString().Replace("-", ""), RegistryValueKind.String);
                    }
                }

                // Очистка BAM (Background Activity Moderator)
                using (RegistryKey? bamKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\bam\State\UserSettings", true))
                {
                    if (bamKey != null)
                    {
                        foreach (string userSid in bamKey.GetSubKeyNames())
                        {
                            using (RegistryKey? userKey = bamKey.OpenSubKey(userSid, true))
                            {
                                if (userKey != null)
                                {
                                    foreach (string valueName in userKey.GetValueNames())
                                    {
                                        if (valueName != "Version" && valueName != "SequenceNumber")
                                        {
                                            userKey.DeleteValue(valueName, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
#pragma warning restore CA1416
                
                // Очистка SRUM (System Resource Usage Monitor) - требует остановки службы
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c net stop DPS /y && del /f /q \"%windir%\\System32\\SRU\\SRUDB.dat\" && net start DPS";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();

                Console.WriteLine("  [+] Телеметрия очищена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Ошибка очистки телеметрии: {ex.Message}");
            }
        }

        private static void DeleteDirectoryContents(string targetDir)
        {
            try
            {
                string[] files = Directory.GetFiles(targetDir);
                string[] dirs = Directory.GetDirectories(targetDir);

                foreach (string file in files)
                {
                    try { File.SetAttributes(file, FileAttributes.Normal); File.Delete(file); } catch { }
                }

                foreach (string dir in dirs)
                {
                    try { Directory.Delete(dir, true); } catch { }
                }
                Console.WriteLine($"  [+] Folder cleaned: {targetDir}");
            }
            catch (Exception)
            {
                Console.WriteLine($"  [-] Failed to fully clean: {targetDir}");
            }
        }
    }
}