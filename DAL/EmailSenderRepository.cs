using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace StoreApi.DAL
{
    public class EmailSenderRepository : IEmailSenderRepository
    {
        private readonly IConfiguration configuration;

        public EmailSenderRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task SendEmailAsync(string memberEmail, string subject, string message)
        {
            var mail = new MimeMessage();
            mail.From.Add(MailboxAddress.Parse(configuration.GetSection("EmailSenderSettings:Email").Value!));
            mail.To.Add(MailboxAddress.Parse(memberEmail));
            mail.Subject = subject;
            mail.Body = new TextPart(TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect(configuration.GetSection("EmailSenderSettings:Host").Value!, Int32.Parse(configuration.GetSection("EmailSenderSettings:Port").Value!), SecureSocketOptions.StartTls);
            smtp.Authenticate(configuration.GetSection("EmailSenderSettings:Email").Value!, configuration.GetSection("EmailSenderSettings:Password").Value!);
            smtp.Send(mail);
            smtp.Disconnect(true);

            return Task.CompletedTask;
        }
    }
}
