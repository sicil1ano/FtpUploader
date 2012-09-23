using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Utilities;

namespace FtpUploader
{
    class ProgramMultiple
    {
        private static string Username { get; set; }
        private static string Password { get; set; }
        private static string[] FilesPath;
        private static string FilePath { get; set; }
        private static string NetworkPath { get; set; }
        //private static Uri addressNetwork;




        static void Main(string[] args)
        {
            Console.WriteLine("---------- FTP UPLOADER ----------");
            System.Threading.Thread.Sleep(500);
           

            Console.WriteLine("\nEnter filepath(s): ");
            Console.WriteLine("\nIf you want to upload multiple files, separate each file path with a comma(,)");
            Console.WriteLine();
            FilePath = Console.ReadLine();
            if (FilePath.Contains(','))
            {
                
                FilesPath = FilePath.Split(',');
                if (FilesPath.Length > 1 && !FilesPath.Contains(""))
                {
                    for (int i = 0; i < FilesPath.Length;i++)
                    {

                        bool fileExists = File.Exists(@FilesPath[i]);
                        while (!fileExists)
                        {
                            Console.WriteLine("\nThe file " + Path.GetFileName(FilesPath[i]) + " doesn't exist. Please enter a valid file path: ");
                            FilesPath[i] = Console.ReadLine();
                            fileExists = File.Exists(FilesPath[i]);
                        }

                    }
                }
            }
            

            do
            {
                Console.WriteLine("\nEnter network path:");
                Console.WriteLine("\n(for example: 127.0.0.1:40/path/)");
                Console.WriteLine();
                NetworkPath = Console.ReadLine();
            }
            while (NetworkPath.Equals(""));


            Username = "";
            Console.WriteLine("\nEnter a valid username: ");
            ConsoleKeyInfo keyUser;

            do
            {
                keyUser = Console.ReadKey(true);
                
                if (keyUser.Key != ConsoleKey.Backspace && keyUser.Key != ConsoleKey.Enter)
                {
                    Username += keyUser.KeyChar;
                    Console.Write(keyUser.KeyChar);
                }

                else
                {
                    if (Username.Length > 0 && keyUser.Key == ConsoleKey.Backspace)
                    {
                        Username = Username.Substring(0, (Username.Length - 1));
                        Console.Write("\b \b");
                    }
                }
                
            }
            while (keyUser.Key != ConsoleKey.Enter || Username.Equals(""));


            Password = "";
            Console.WriteLine();
            Console.WriteLine("\nEnter a valid password:");
            ConsoleKeyInfo key;

            /* masking password */
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    Password += key.KeyChar;
                    Console.Write("*");

                }
                else
                {
                    
                    if (Password.Length > 0 && key.Key == ConsoleKey.Backspace)
                    {
                        Password = Password.Substring(0, (Password.Length - 1));


                        Console.Write("\b \b");
                    }
                    
                }
            }
            while (key.Key != ConsoleKey.Enter || Password.Equals(""));
            
            


            Console.WriteLine();




            NetworkCredential netcred = new NetworkCredential(Username, Password);

            /* making sure the object is not null, is not empty, and the elements are not empty strings */
            if (FilesPath != null && FilesPath.Length != 0 && !FilesPath.Any(p=> p.Equals("")))
            {
                bool[] results = new bool[FilesPath.Length];
                
                results.Populate(false);
                try
                {
                    results = FtpHelper.UploadFiles(FilesPath, NetworkPath, netcred);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred.");
                    Console.WriteLine("\nException: " + ex.Message);
                    
                    if (ex.InnerException != null)
                        Console.WriteLine("\nInnerException: " + ex.InnerException);
                    Console.WriteLine("\nPress any key to exit ...");
                    Console.ReadKey();
                    Environment.Exit(666);
                }
                if (!results.Any(p => !p))
                {
                    Console.WriteLine("\n   ---------------   ");
                    Console.WriteLine("\nFTP Upload successfully completed.");
                }
                else
                    Console.WriteLine("\nFTP Upload was not completed successfully");
            }
            else
            {
                if(FilePath.Contains(','))
                    FilePath = FilePath.Replace(",", "");
                bool result = false;
                try
                {
                    result = FtpHelper.UploadFiles(FilePath, NetworkPath, netcred);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred.");
                    Console.WriteLine("\nException: " + ex.Message);
                    
                    if (ex.InnerException != null)
                        Console.WriteLine("\nInnerException: " + ex.InnerException);
                    Console.WriteLine("\nPress any key to exit ...");
                    Console.ReadKey();
                    Environment.Exit(666);
                }
                if (result)
                {
                    Console.WriteLine("\n   ---------------   ");
                    Console.WriteLine("\nFTP Upload successfully completed.");
                }
                else
                    Console.WriteLine("\nFTP Upload was not completed successfully");
            }
            Console.WriteLine("\nPress any key to exit ...");
            Console.ReadKey();
        }

    }
}
