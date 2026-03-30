using System;
using Microsoft.Win32;

namespace PhantomCore
{
    public class MacSpoofer
    {
        private const string REGISTRY_PATH = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}";

        public static void SpoofMacAddress()
        {
            Console.WriteLine("  [*] Changing MAC addresses...");
            
            try
            {
#pragma warning disable CA1416
                using (RegistryKey? networkAdaptersKey = Registry.LocalMachine.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (networkAdaptersKey == null)
                    {
                        Console.WriteLine("  [-] Failed to open adapters registry key.");
                        return;
                    }

                    foreach (string adapterKeyName in networkAdaptersKey.GetSubKeyNames())
                    {
                        if (adapterKeyName == "Properties") continue;

                        using (RegistryKey? adapterKey = networkAdaptersKey.OpenSubKey(adapterKeyName, true))
                        {
                            if (adapterKey == null) continue;

                            object? busType = adapterKey.GetValue("BusType");
                            string? driverDesc = adapterKey.GetValue("DriverDesc") as string;
                            string? netCfgInstanceId = adapterKey.GetValue("NetCfgInstanceId") as string;

                            // We only want to target physical-ish adapters (PCI, USB) or those that have NetCfgInstanceId
                            if (busType != null && driverDesc != null && netCfgInstanceId != null)
                            {
                                string newMac = GenerateRandomMac();
                                adapterKey.SetValue("NetworkAddress", newMac, RegistryValueKind.String);
                                Console.WriteLine($"  [+] MAC address for {driverDesc} successfully changed to {newMac}");
                            }
                        }
                    }
                }
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [!] Error changing MAC address: {ex.Message}");
            }
        }

        private static string GenerateRandomMac()
        {
            Random random = new Random();
            byte[] buffer = new byte[6];
            random.NextBytes(buffer);
            
            // Set first octet to valid values for unicast, locally administered
            buffer[0] = (byte)(buffer[0] & 0xFE); // ensure unicast
            buffer[0] = (byte)(buffer[0] | 0x02); // ensure locally administered
            
            // Some specific formatting for Windows Registry (no colons)
            return $"{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2}{buffer[4]:X2}{buffer[5]:X2}";
        }
    }
}