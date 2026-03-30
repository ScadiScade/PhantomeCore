# PhantomCore

PhantomCore is a powerful system utility written in C# (.NET) designed to manage hardware identifiers (HWID) and clean system traces. 

## Features

- **MAC Spoofer**: Easily change or randomize the MAC addresses of your network adapters.
- **Volume ID Spoofer**: Modify the Volume Serial Numbers of your logical partitions.
- **System ID Spoofer**: Manage and spoof various system-level identifiers and registry keys.
- **Trace Cleaner**: Clean up system traces, temporary files, deep system logs, and tracking artifacts.
- **Profile Manager**: Save and load different hardware identity profiles easily.
- **Kernel Driver**: Includes a kernel-mode driver (`PhantomCoreDriver.inf` / `Driver.c`) for low-level system operations.

## Requirements

- Windows 10 / 11 (x64)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/) (or newer)
- **Administrator privileges** are strictly required to run most of the spoofing and cleaning operations.
- Windows Test Mode may be required if loading the unsigned custom kernel driver.

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/PhantomCore.git
   ```
2. Open the project in Visual Studio or JetBrains Rider.
3. Build the solution in `Release` mode.
4. Run `PhantomCore.exe` as Administrator.

## Disclaimer

**For Educational Purposes Only.** 
This tool modifies core system identifiers and deletes system traces. Use it at your own risk. The developers are not responsible for any damage to your operating system, broken software licenses, hardware issues, or violation of third-party Terms of Service resulting from the use of this software.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
