using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Security.AccessControl;

namespace IIS_WebsiteFolderPermissionSetter
{
    class Program
    {
        private static string logFile = "log.txt";

        static void Main(string[] args)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                try
                {
                    if (serverManager != null)
                    {
                        Console.WriteLine("Set Application pool identity folderpermissions (Modify) to all websites? Y/N");
                        if (Console.ReadLine().ToUpper() == "Y")
                        {
                            using (StreamWriter sw = File.AppendText(logFile))
                            {
                                sw.WriteLine("Setting permissions for directories at " + DateTime.Now);
                                sw.WriteLine();
                            }

                            foreach (Site site in serverManager.Sites)
                            {
                                foreach (var application in site.Applications)
                                {
                                    string directoryPath = application.VirtualDirectories["/"].PhysicalPath;

                                    if (Directory.Exists(directoryPath))
                                    {
                                        using (StreamWriter sw = File.AppendText(logFile))
                                        {
                                            sw.WriteLine(application.ApplicationPoolName + ": " + directoryPath);
                                        }

                                        Console.WriteLine(application.ApplicationPoolName + ": " + directoryPath);

                                        DirectoryInfo info = new DirectoryInfo(directoryPath);
                                        DirectorySecurity ds = info.GetAccessControl();

                                        // Sets ACE to folder, sub-folders and files
                                        FileSystemAccessRule appIdentityRule = new FileSystemAccessRule(
                                                @"IIS APPPOOL\" + application.ApplicationPoolName,
                                                FileSystemRights.Modify,
                                                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                PropagationFlags.None,
                                                AccessControlType.Allow
                                            );

                                        ds.AddAccessRule(appIdentityRule);
                                        info.SetAccessControl(ds);
                                    } else
                                    {
                                        using (StreamWriter sw = File.AppendText(logFile))
                                        {
                                            sw.WriteLine("DIRECTORY MISSING FOR " + application.ApplicationPoolName + ": " + directoryPath);
                                        }

                                        Console.WriteLine("DIRECTORY MISSING FOR " + 
                                            application.ApplicationPoolName + ": " + directoryPath);
                                    }
                                }
                            }
                            using (StreamWriter sw = File.AppendText(logFile))
                            {
                                sw.WriteLine();
                            }
                        }                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    using (StreamWriter sw = File.AppendText(logFile))
                    {
                        sw.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
