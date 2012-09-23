using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Utilities;
using System.Threading;

namespace FtpUploader
{
    static class FtpHelper
    {

        private static FtpWebRequest request;
        private static FtpWebResponse response;

        private static Uri UriPath(string network)
        {
            return new Uri(string.Format("ftp://{0}", network));
        }

        /* 1st version of the method UploadFiles, used to upload one file.  (KeepAlive = false) */

        public static bool UploadFiles(string filePath, string networkPath, NetworkCredential credentials)
        {

            bool resultUpload = false;
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("\nInitializing Network Connection..");


            

            
            request = (FtpWebRequest)WebRequest.Create(UriPath(networkPath) + Path.GetFileName(filePath));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UsePassive = true;
            request.KeepAlive = false; 
            request.Credentials = credentials;

            /* set ssl protection? */
            //request.EnableSsl = true;
            using (Stream requestStream = request.GetRequestStream())
            {

                System.Threading.Thread.Sleep(500);
                Console.WriteLine("\nReading the file " + Path.GetFileName(filePath) + "...");
                using (StreamReader sourceStream = new StreamReader(filePath))
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    request.ContentLength = fileContents.Length;

                    

                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                }
            }

            

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("\nUpload of " + Path.GetFileName(filePath) + " file complete.");
                Console.WriteLine("Request status: {0}", response.StatusDescription);
                if (response.StatusDescription.Equals("226 Transfer complete.\r\n"))
                    resultUpload = true;
                response.Close();

            }


            Console.WriteLine("\nClosing connection to {0}...", string.Format("ftp://{0}", networkPath));
            return resultUpload;
        }

        /* 2nd version of the method UploadFiles, used to upload more than one file. You can notice the same request is opened until all the files are uploaded. (KeepAlive = true) */

        public static bool[] UploadFiles(string[] filePath, string networkPath, NetworkCredential credentials)
        {
            bool[] resultUpload = new bool[filePath.Length];
            resultUpload.Populate(false);
            Console.WriteLine("\nInitializing Network Connection..");
            System.Threading.Thread.Sleep(500);
            for (int i = 0; i < filePath.Length; i++)
            {
                
                string currentFile = filePath[i].ToString();
                
                request = (FtpWebRequest)WebRequest.Create(UriPath(networkPath) + Path.GetFileName(currentFile));
                
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UsePassive = true;
                
                request.KeepAlive = true;
                request.Credentials = credentials;

                /* set ssl protection? */
                //request.EnableSsl = true;
                using (Stream requestStream = request.GetRequestStream())
                {

                    System.Threading.Thread.Sleep(500);
                   
                    byte[] fileContents;
                    using (FileStream fs = new FileStream(currentFile, FileMode.Open))
                    {
                        System.Threading.Thread.Sleep(500);
                        Console.WriteLine("\nReading the file " + Path.GetFileName(currentFile) + "...");
                        fileContents = new byte[fs.Length];
                        int numBytesRead = 0;
                        int numBytesToRead = (int)fs.Length;
                        while (numBytesToRead > 0)
                        {
                            int n = fs.Read(fileContents, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;

                        }
                        numBytesToRead = fileContents.Length;
                        
                        fs.Close();
                        request.ContentLength = fileContents.Length;

                        
                    }
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                }

                response = (FtpWebResponse)request.GetResponse();
                

                    System.Threading.Thread.Sleep(500);
                    Console.WriteLine("\nUpload of the file " + Path.GetFileName(filePath[i]) + " complete.");
                    Console.WriteLine("Request status: {0}", response.StatusDescription);
                    if (response.StatusDescription.Equals("226 Transfer complete.\r\n"))
                        resultUpload[i] = true;

                    


                    
                
            }
            Console.WriteLine("\nClosing connection to {0}...", string.Format("ftp://{0}", networkPath));
            
            response.Close();
            ((IDisposable)response).Dispose();

                /* before I used this implementation to close and dispose the response */

                //    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        Console.WriteLine("\nUpload of " + Path.GetFileName(filePath[i]) + " file complete.");
                //        Console.WriteLine("Request status: {0}", response.StatusDescription);
                //        if (response.StatusDescription.Equals("226 Transfer complete.\r\n"))
                //            resultUpload[i] = true;

                //        response.Close();
                //    }
                //    request = null;
                //}


                return resultUpload;
            }
        }
}