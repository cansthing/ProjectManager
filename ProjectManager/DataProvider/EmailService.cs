using MailKit.Security;
using MimeKit;
using ProjectManager.Helper;
using ProjectManager.Model;

using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace ProjectManager.DataProvider
{
    class EmailService : ISendNotification
    {
        public async Task<bool> Send(User to, string subject, StringBuilder content)
        {
            if (!await IsValid(to))
            {
                MessageBox.Show("Die Email vom Benutzer ist nicht bestätigt.");
                return false;
            }
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Projekt Manager", "josua@l-en.de"));
                email.To.Add(new MailboxAddress(to.Fullname, to.Email));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = content.ToString() };

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync("smtp.goneo.de", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("josua@l-en.de", EncryptionHelper.DecryptString("oNwyFq3fcmloDJZv9yUA9LaDVv/XlRMlpsnz3Zs1fdc="));
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> IsValid(User user)
        {
            await Task.Delay(0);
            return true;
        }
        public async Task<bool> AssignmentCreated(Assignment assignment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HtmlHelper.TableStyle());
            sb.Append(HtmlHelper.Greeting(assignment.User));
            sb.Append("<h3>Du hast eine neue Aufgabe</h3>");
            sb.Append(HtmlHelper.Assignment(assignment));
            sb.Append(HtmlHelper.CurrentAssignments(await ObjectRepository.DataProvider.GetAssignments(assignment.User)));
            sb.Append(HtmlHelper.Footer());
            await Send(assignment.User, "Projekt Manager : Aufgaben : Test", sb);
            return true;
        }

        public async Task<bool> AssignmentDeleted(Assignment assignment)
        {
            await Task.Delay(0);
            return false;
        }

        public async Task<bool> AssignmentUpdated(Assignment assignment)
        {
            return false;
        }
        public async Task AllAssignments(ObservableCollection<Assignment> assignments)
        {
            
        }
    }
}

