using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace FtpUploader
{
    class Program
    {
        private static string Username { get; set; }
        private static string Password { get; set; }
        private static string[] FilesPath;
        private static string NetworkPath { get; set; }
        private static Uri addressNetwork;


        

        static void Main(string[] args)
        {
            Console.WriteLine("---------- FTP UPLOADER ----------");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("\nEnter filepath(s): ");
            FilesPath = Console.ReadLine();
            if (!FilesPath.Equals(""))
            {
                bool fileExists = File.Exists(FilesPath);
                while (fileExists == false)
                {
                    Console.WriteLine("\nThis file doesn't exist. Please enter a valid file path:");
                    FilesPath = Console.ReadLine();
                    fileExists = File.Exists(FilesPath);
                }
            }

            bool uriCheck = true;
            
            do
            {
                Console.WriteLine("\nEnter network path:");
                Console.WriteLine("\n(for example: ftp://127.0.0.1)");
                Console.WriteLine();
                NetworkPath = Console.ReadLine();
                try
                {
                    addressNetwork = new Uri(NetworkPath);
                    uriCheck = Uri.CheckSchemeName(addressNetwork.Scheme);
                    
                    if (!uriCheck)
                    {
                        Console.WriteLine("\nNetwork path not valid. Review the path. ");
                        Console.WriteLine("\nThe default FTP port is 21. \n\nIf yours is not, enter the port number with a colon (:), after the network path.");
                        Console.WriteLine("\n(for example: 'ftp://1.1.1.1:90'");
                    }
                }
                catch (Exception uriex)
                {
                    Console.WriteLine("Error occurred.");
                    Console.WriteLine("\nException: " + uriex.Message);
                    if (uriex.InnerException != null)
                        Console.WriteLine("\nInnerException: " + uriex.InnerException);
                    Console.WriteLine("\nPress any key to exit ....");
                    Console.ReadKey();
                    Environment.Exit(666);
                }
            }
            while (!uriCheck);

            Console.WriteLine("\nEnter your username");
            Username = Console.ReadLine();
            Console.WriteLine("\nEnter your password");
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
                        
                        //Password = Password.TrimEnd(Password[Password.Length-1]);
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            
            
            Console.WriteLine();

            
            

            NetworkCredential netcred = new NetworkCredential(Username, Password);
            bool result = false;
            try
            {
                result = FtpHelper.UploadFiles(FilesPath, addressNetwork, netcred);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred.");
                Console.WriteLine("\nException: " + ex.Message);
                //Console.WriteLine("\nStack Trace: "+ex.StackTrace);
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
            Console.WriteLine("\nPress any key to exit ...");
            Console.ReadKey();
        }

    }
}
