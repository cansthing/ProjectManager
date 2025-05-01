using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace ProjectManager
{
    public class Email
    {
        //public static void Send()
        //{
        //    MailMessage mail = new MailMessage();
        //    SmtpClient smtpServer = new SmtpClient("smtp.office365.com");

        //    mail.From = new MailAddress("warenwirtschaft-cvl@outlook.com");
        //    mail.To.Add("josua@l-en.de");
        //    mail.Subject = "Automatische E-Mail";
        //    mail.Body = "Dies ist eine automatisch gesendete E-Mail.";

        //    smtpServer.Port = 587;
        //    smtpServer.Credentials = new NetworkCredential("warenwirtschaft-cvl@outlook.com", "WarenwirtschaftCVL2015!");
        //    smtpServer.EnableSsl = true;

        //    try
        //    {
        //        smtpServer.Send(mail);
        //        MessageBox.Show("E-Mail erfolgreich gesendet!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Fehler: " + ex.Message);
        //    }
        //}
        public static void Senden()
        {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("PM", "warenwirtschaft-cvl@outlook.com"));
                email.To.Add(new MailboxAddress("Josua Löwen", "josua@l-en.de"));
                email.Subject = "Automatische E-Mail mit MailKit";
            email.Body = new TextPart("html")
            {
                Text = "Dies ist eine automatisch gesendete E-Mail mit MailKit.",
                
            };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("warenwirtschaft-cvl@outlook.com", "WarenwirtschaftCVL2015!");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                Console.WriteLine("E-Mail erfolgreich gesendet!");
            }
        
    }
}