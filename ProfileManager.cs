using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace PhantomCore
{
    public class ProfileManager
    {
        private const string ProfileFileName = "hwid_profile.json";

        public class HWIDProfile
        {
            public string? MachineGuid { get; set; }
            public string? ProductId { get; set; }
            public string? ComputerName { get; set; }
            public string? OriginalMacs { get; set; } // В реальном приложении здесь был бы словарь Adapter->MAC
            public DateTime BackupDate { get; set; }
        }

        public static void BackupCurrentState()
        {
            Console.WriteLine("  [*] Creating backup of current IDs...");

            try
            {
                HWIDProfile profile = new HWIDProfile
                {
                    MachineGuid = GetRegistryValue(@"SOFTWARE\Microsoft\Cryptography", "MachineGuid"),
                    ProductId = GetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductId"),
                    ComputerName = Environment.MachineName,
                    BackupDate = DateTime.Now
                };

                string jsonString = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ProfileFileName, jsonString);

                Console.WriteLine($"  [+] Профиль успешно сохранен в {ProfileFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Ошибка при создании профиля: {ex.Message}");
            }
        }

        public static void RestoreProfile()
        {
            Console.WriteLine("  [*] Восстановление из профиля...");

            if (!File.Exists(ProfileFileName))
            {
                Console.WriteLine($"  [-] Файл профиля {ProfileFileName} не найден.");
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(ProfileFileName);
                HWIDProfile? profile = JsonSerializer.Deserialize<HWIDProfile>(jsonString);

                if (profile != null)
                {
                    if (!string.IsNullOrEmpty(profile.MachineGuid))
                        SetRegistryValue(@"SOFTWARE\Microsoft\Cryptography", "MachineGuid", profile.MachineGuid);
                    
                    if (!string.IsNullOrEmpty(profile.ProductId))
                        SetRegistryValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductId", profile.ProductId);

                    Console.WriteLine($"  [+] Restored. Backup date: {profile.BackupDate}");
                    Console.WriteLine("  [!] Warning: MAC addresses and Computer Name require manual restoration or reboot.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [-] Error restoring profile: {ex.Message}");
            }
        }

        private static string? GetRegistryValue(string path, string keyName)
        {
#pragma warning disable CA1416
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(path))
            {
                return key?.GetValue(keyName)?.ToString();
            }
#pragma warning restore CA1416
        }

        private static void SetRegistryValue(string path, string keyName, string value)
        {
#pragma warning disable CA1416
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(path, true))
            {
                if (key != null)
                {
                    key.SetValue(keyName, value, RegistryValueKind.String);
                    Console.WriteLine($"  [+] Restored {keyName}: {value}");
                }
            }
#pragma warning restore CA1416
        }
    }
}