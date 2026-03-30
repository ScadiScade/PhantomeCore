using System;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace PhantomCore
{
    public class NetworkUtils
    {
        public static void FlushDns()
        {
            Console.WriteLine("  [*] Flushing DNS cache...");
            ExecuteCommand("ipconfig", "/flushdns");
            Console.WriteLine("  [+] DNS cache successfully flushed.");
        }

        public static void RestartNetworkAdapters()
        {
            Console.WriteLine("  [*] Restarting network adapters...");
            
            try
            {
#pragma warning disable CA1416
                SelectQuery query = new SelectQuery("Win32_NetworkAdapter", "NetEnabled='True'");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject adapter in searcher.Get())
                    {
                        string? name = adapter["Name"] as string;
                        Console.WriteLine($"  [*] Disabling: {name}");
                        adapter.InvokeMethod("Disable", null);
                        
                        Thread.Sleep(2000); // Wait a bit
                        
                        Console.WriteLine($"  [*] Enabling: {name}");
                        adapter.InvokeMethod("Enable", null);
                    }
                }
#pragma warning restore CA1416
                Console.WriteLine("  [+] Adapters successfully restarted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Error restarting adapters (via WMI): {ex.Message}");
                Console.WriteLine("  [*] Attempting to restart via netsh...");
                // Fallback method
                ExecuteCommand("cmd", "/c ipconfig /release && ipconfig /renew");
            }
        }

        private static void ExecuteCommand(string fileName, string arguments)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
            catch { }
        }
    }
}