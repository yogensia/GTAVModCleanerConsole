using System;
using System.IO;
using Microsoft.Win32;

namespace GTAVModCleanerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get installation folder from registry
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            string GTAVLocation = FindByDisplayName(regKey, "Grand Theft Auto V");

            // Show GTAV folder to user
            Console.WriteLine("GTAV found in: \"{0}\".", GTAVLocation);

            WaitUserInput();

            // Define known files/folders that we won't move when cleaning the GTAV folder
            string[] knownFiles = {
                "ReadMe\\",
                "update\\",
                "x64\\",
                "bink2w64.dll",
                "commandline.txt",
                "common.rpf",
                "d3dcompiler_46.dll",
                "d3dcsx_46.dll",
                "GFSDK_ShadowLib.win64.dll",
                "GFSDK_TXAA.win64.dll",
                "GFSDK_TXAA_AlphaResolve.win64.dll",
                "GPUPerfAPIDX11-x64.dll",
                "GTA5.exe",
                "GTAVLauncher.exe",
                "PlayGTAV.exe",
                "NvPmApi.Core.win64.dll",
                "version.txt",
                "steam_appid.txt",
                "x64a.rpf",
                "x64b.rpf",
                "x64c.rpf",
                "x64d.rpf",
                "x64e.rpf",
                "x64f.rpf",
                "x64g.rpf",
                "x64h.rpf",
                "x64i.rpf",
                "x64j.rpf",
                "x64k.rpf",
                "x64l.rpf",
                "x64m.rpf",
                "x64n.rpf",
                "x64o.rpf",
                "x64p.rpf",
                "x64q.rpf",
                "x64r.rpf",
                "x64s.rpf",
                "x64t.rpf",
                "x64u.rpf",
                "x64v.rpf",
                "x64w.rpf"
            };

            // Get MODs Backup folder
            string MODLocation = Path.GetDirectoryName(GTAVLocation);
            MODLocation = Path.Combine(MODLocation, "Grand Theft Auto V Mods BAK");

            // Check that MODs Backup folder doesn't already exist
            if (Directory.Exists(MODLocation))
            {
                Console.WriteLine("Mod Backup folder \"{0}\" already exists!", MODLocation);
                Console.WriteLine("Delete/rename it first and run this program again.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Mods will be moved to \"{0}\".", MODLocation);
            }

            WaitUserInput();

            // Final Confirmation
            Console.WriteLine("Mod files in \"{0}\" will be moved to \"{1}\". Are you sure?", GTAVLocation, MODLocation);

            WaitUserInput();

            // Move everything to MODs Backup folder
            try
            {
                Directory.Move(GTAVLocation, MODLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Recreate Installation folder
            Directory.CreateDirectory(GTAVLocation);

            // Move back known files to the installation folder
            string[] MODLocationFiles = (string[])knownFiles.Clone();
            string[] GTAVLocationFiles = (string[])knownFiles.Clone();

            for (var i = 0; i < knownFiles.Length; i++)
            {
                MODLocationFiles[i] = Path.Combine(MODLocation, knownFiles[i]);
                GTAVLocationFiles[i] = Path.Combine(GTAVLocation, knownFiles[i]);

                // Check if is a file or a directory
                FileAttributes attr = File.GetAttributes(MODLocationFiles[i]);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine("{0} is directory", MODLocationFiles[i]);
                    try
                    {
                        Directory.Move(MODLocationFiles[i], GTAVLocationFiles[i]); // Try to move
                        Console.WriteLine("Moving {0} to {1}", MODLocationFiles[i], GTAVLocationFiles[i]);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex); // Write error
                    }
                }
                else
                {
                    Console.WriteLine("{0} is file", MODLocationFiles[i]);
                    try
                    {
                        File.Move(MODLocationFiles[i], GTAVLocationFiles[i]); // Try to move
                        Console.WriteLine("Moving {0} to {1}", MODLocationFiles[i], GTAVLocationFiles[i]);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex); // Write error
                    }
                }
            }

            // All done and exit
            Console.WriteLine();
            Console.WriteLine("ALL DONE, press any key to exit...");
            Console.ReadKey();

        } // main

        // Returns installation folder of an application
        static string FindByDisplayName(RegistryKey parentKey, string name)
        {
            string[] nameList = parentKey.GetSubKeyNames();
            for (int i = 0; i < nameList.Length; i++)
            {
                RegistryKey regKey = parentKey.OpenSubKey(nameList[i]);
                try
                {
                    if (regKey.GetValue("DisplayName").ToString() == name)
                    {
                        return regKey.GetValue("InstallLocation").ToString();
                    }
                }
                catch { }
            }
            return "";
        } // FindByDisplayName

        static void WaitUserInput()
        {
            // Exit on Esc key pressed, otherwise continue
            Console.WriteLine("Press ESC to cancel or any other key to continue...");
            ConsoleKeyInfo cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
                return;
            }
            else
            {
                Console.WriteLine();
                return;
            }

        } // WaitUserInput
    } // class
} // namespace