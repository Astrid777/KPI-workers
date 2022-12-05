using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CustomIdentityApp
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //настройки почты
            //webapp1
            var Path = Directory.GetCurrentDirectory();

            var fullPath = Path + "\\settingsMail.txt";

            string settings = "";

            try
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    settings= sr.ReadToEnd();
                    Console.WriteLine("1  " + settings);
                }
            }
            catch
            {
                //Console.WriteLine("ош " + fullPath);
            }

             Mailer mail = new Mailer();
             
            var mailSettings = mail.Parse(settings);

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Сайт оценка сотрудников", mailSettings.login));
            emailMessage.To.Add(new MailboxAddress("", email));
            
            //тема
            emailMessage.Subject = subject;
            
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(mailSettings.server, mailSettings.port, mailSettings.ssl);
                await client.AuthenticateAsync(mailSettings.login, mailSettings.password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
