using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace PhantomCore
{
    public class SystemIdSpoofer
    {
        private enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetComputerNameEx(COMPUTER_NAME_FORMAT NameType, string lpBuffer);

        public static void SpoofMachineGuid()
        {
            Console.WriteLine("  [*] Changing Machine GUID...");
            try
            {
#pragma warning disable CA1416
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", true))
                {
                    if (key != null)
                    {
                        string newGuid = Guid.NewGuid().ToString();
                        key.SetValue("MachineGuid", newGuid, RegistryValueKind.String);
                        Console.WriteLine($"  [+] Machine GUID changed to: {newGuid}");
                    }
                    else
                    {
                        Console.WriteLine("  [-] Failed to open registry key for Machine GUID.");
                    }
                }
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Error changing Machine GUID: {ex.Message}");
            }
        }

        public static void SpoofProductId()
        {
            Console.WriteLine("  [*] Changing Product ID...");
            try
            {
#pragma warning disable CA1416
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true))
                {
                    if (key != null)
                    {
                        string newProductId = GenerateRandomProductId();
                        key.SetValue("ProductId", newProductId, RegistryValueKind.String);
                        Console.WriteLine($"  [+] Product ID changed to: {newProductId}");
                    }
                    else
                    {
                        Console.WriteLine("  [-] Failed to open registry key for Product ID.");
                    }
                }
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Error changing Product ID: {ex.Message}");
            }
        }

        public static void SpoofComputerName()
        {
            Console.WriteLine("  [*] Changing Computer Name...");
            try
            {
                string newName = "DESKTOP-" + GenerateRandomString(7);

                bool result1 = SetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsHostname, newName);
                bool result2 = SetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalNetBIOS, newName);

                if (result1 && result2)
                {
                    Console.WriteLine($"  [+] Computer Name changed to: {newName} (Reboot required)");
                }
                else
                {
                    Console.WriteLine($"  [-] Error changing Computer Name. Error code: {Marshal.GetLastWin32Error()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Error: {ex.Message}");
            }
        }

        private static string GenerateRandomProductId()
        {
            Random rnd = new Random();
            return $"{rnd.Next(10000, 99999)}-{rnd.Next(10000, 99999)}-{rnd.Next(10000, 99999)}-{rnd.Next(10000, 99999)}";
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }
}