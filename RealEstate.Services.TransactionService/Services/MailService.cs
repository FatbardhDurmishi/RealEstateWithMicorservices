using System.Net.Mail;
using System.Net;

namespace RealEstate.Services.TransactionService.Services
{
    public class MailService
    {
        private IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var outlook = _configuration.GetSection("MailSettings");
            var senderEmail = outlook.GetValue<string>("SenderEmail");
            var password = outlook.GetValue<string>("Password");
            var server = outlook.GetValue<string>("Server");
            var port = outlook.GetValue<int>("Port");

            using (SmtpClient client = new SmtpClient(server, port))
            using (MailMessage message = new MailMessage(senderEmail, email))
            {
                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;

                client.Credentials = new NetworkCredential(senderEmail, password);
                client.EnableSsl = true;

                try
                {
                    await client.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine(ex.ToString());
                    // Handle the exception as needed
                }
            }

        }
    }
}
