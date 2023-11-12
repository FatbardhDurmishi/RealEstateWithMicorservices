using System.Net.Mail;
using System.Net;

namespace RealEstate.Services.TransactionService.Services
{
    public class EmailService
    {
        private IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(string email, string subject, string htmlMessage)
        {
            var outlook = _configuration.GetSection("Outlook");
            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress(outlook.GetValue<string>("Email"));
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = htmlMessage;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                Credentials = new NetworkCredential(outlook.GetValue<string>("Email"), outlook.GetValue<string>("Password")),
                EnableSsl = true
                // specify whether your host accepts SSL connections
            };
            // code in brackets above needed if authentication required
            try
            {
                client.SendAsync(message,null);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
