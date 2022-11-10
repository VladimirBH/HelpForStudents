using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class EmailService
    {
        public static async Task SendEmailAsync(string emailFrom, string password, string emailTo, string subject, string message)
        {
            var emailMessage = new MimeMessage();
 
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", emailFrom));
            emailMessage.To.Add(new MailboxAddress("", emailTo));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
             
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.AuthenticateAsync(emailFrom, password);
                await client.SendAsync(emailMessage);
 
                await client.DisconnectAsync(true);
            }
        }
    }
}