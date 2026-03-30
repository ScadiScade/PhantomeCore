using System;
using System.Threading;

namespace PhantomCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PhantomCore | Advanced Identity Spoofer & Trace Cleaner";
            // Set UTF-8 encoding to support nice symbols
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            while (true)
            {
                Console.Clear();
                DrawHeader();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n  [ IDENTIFICATION ]");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  [1] Spoof MAC Address");
                Console.WriteLine("  [2] Spoof Machine GUID");
                Console.WriteLine("  [3] Spoof Product ID");
                Console.WriteLine("  [4] Spoof Computer Name");
                Console.WriteLine("  [5] Spoof Volume ID");
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n  [ NETWORK & CLEANING ]");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  [6] Flush DNS Cache");
                Console.WriteLine("  [7] Restart Network Adapters");
                Console.WriteLine("  [8] Deep Trace Cleaning (Temp, Cache, Logs, BAM, AdvID)");
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n  [ PROFILING ]");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  [9] Create Backup of current IDs");
                Console.WriteLine("  [10] Restore IDs from Backup");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  [11] ☢ APPLY ALL (Full spoof and clean) ☢");
                
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("  [0] Exit\n");
                
                Console.ResetColor();
                Console.Write("  » Select action: ");
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                string? choice = Console.ReadLine();
                Console.ResetColor();
                Console.WriteLine();

                // Require Admin privileges for registry changes
                if (!IsAdministrator() && choice != "0" && choice != "6")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  [!] КРИТИЧЕСКАЯ ОШИБКА: Требуются права Администратора!");
                    Console.WriteLine("      Пожалуйста, перезапустите программу от имени Администратора.");
                    Console.ResetColor();
                }
                else
                {
                    ExecuteChoice(choice);
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n  Нажмите любую клавишу для возврата в меню...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private static void DrawHeader()
        {
            string[] logo = new string[]
            {
                @"    ____  _                 __                 ______               ",
                @"   / __ \/ /_  ____ _____  / /_____  ____ ___ / ____/___  ________  ",
                @"  / /_/ / __ \/ __ `/ __ \/ __/ __ \/ __ `__ \/ /   / __ \/ ___/ _ \ ",
                @" / ____/ / / / /_/ / / / / /_/ /_/ / / / / / / /___/ /_/ / /  /  __/",
                @"/_/   /_/ /_/\__,_/_/ /_/\__/\____/_/ /_/ /_/\____/\____/_/   \___/ "
            };

            ConsoleColor[] colors = { 
                ConsoleColor.DarkMagenta, 
                ConsoleColor.Magenta, 
                ConsoleColor.DarkRed, 
                ConsoleColor.Red, 
                ConsoleColor.DarkYellow 
            };

            Console.WriteLine();
            for (int i = 0; i < logo.Length; i++)
            {
                Console.ForegroundColor = colors[i % colors.Length];
                Console.WriteLine("  " + logo[i]);
            }
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ──────────────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("    v1.0.0 | Advanced Anti-Fingerprint Engine | User-Mode Edition");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ──────────────────────────────────────────────────────────────────");
            Console.ResetColor();
        }

        private static void ExecuteChoice(string? choice)
        {
            switch (choice)
            {
                case "1":
                    MacSpoofer.SpoofMacAddress();
                    break;
                case "2":
                    SystemIdSpoofer.SpoofMachineGuid();
                    break;
                case "3":
                    SystemIdSpoofer.SpoofProductId();
                    break;
                case "4":
                    SystemIdSpoofer.SpoofComputerName();
                    break;
                case "5":
                    VolumeIdSpoofer.SpoofVolumeId();
                    break;
                case "6":
                    NetworkUtils.FlushDns();
                    break;
                case "7":
                    NetworkUtils.RestartNetworkAdapters();
                    break;
                case "8":
                    TraceCleaner.DeepClean();
                    break;
                case "11":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  [*] Инициализация протокола 'Phantom'...");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    
                    MacSpoofer.SpoofMacAddress();
                    SystemIdSpoofer.SpoofMachineGuid();
                    SystemIdSpoofer.SpoofProductId();
                    SystemIdSpoofer.SpoofComputerName();
                    VolumeIdSpoofer.SpoofVolumeId();
                    NetworkUtils.FlushDns();
                    TraceCleaner.DeepClean();
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n  [+] ПРОТОКОЛ ЗАВЕРШЕН. Для применения изменений (Имя ПК, Volume ID)");
                    Console.WriteLine("      требуется полная перезагрузка системы.");
                    Console.ResetColor();
                    break;
                case "9":
                    ProfileManager.BackupCurrentState();
                    break;
                case "10":
                    ProfileManager.RestoreProfile();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  [-] Unknown command.");
                    Console.ResetColor();
                    break;
            }
        }

#pragma warning disable CA1416
        private static bool IsAdministrator()
        {
            using (System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
#pragma warning restore CA1416
    }
}