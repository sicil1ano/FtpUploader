using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace FtpUploader
{
    static class FtpHelper
    {
        
        private static FtpWebRequest request;
        

        public static bool UploadFiles(string filePath, Uri networkPath, NetworkCredential credentials)
        {
            bool resultUpload = false;
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("\nInitializing Network Connection..");
            

            /* si potrebbe impostare da riga di comando il keep alive, la passive mode ed il network path */
            /* progress bar? */
            
                //request = (FtpWebRequest)WebRequest.Create(networkPath + @"/" + Path.GetFileName(filePath));
                request = (FtpWebRequest) WebRequest.Create(networkPath + @"/"  + Path.GetFileName(filePath));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UsePassive = true;
                request.KeepAlive = false; /* è necessario? */
                request.Credentials = credentials;
            
            /* è necessario impostare la protezione ssl? */
            //request.EnableSsl = true;
                using (Stream requestStream = request.GetRequestStream())
                {

                    System.Threading.Thread.Sleep(500);
                    Console.WriteLine("\nReading the file to transfer...");
                    using (StreamReader sourceStream = new StreamReader(filePath))
                    {
                        byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                        sourceStream.Close();
                        request.ContentLength = fileContents.Length;

                        System.Threading.Thread.Sleep(500);
                        Console.WriteLine("\nTransferring the file..");

                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();
                    }
                }

            Console.WriteLine("\nClosing connection...");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("\nUpload of " + Path.GetFileName(filePath) + " file complete.");
                Console.WriteLine("Request status: {0}", response.StatusDescription);
                if(response.StatusDescription.Equals("226 Transfer complete.\r\n"))
                    resultUpload = true;
                response.Close();
            }

            

            return resultUpload;
        }
    }
}
