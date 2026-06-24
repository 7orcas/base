
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Scriban;
using GC = Backend.GlobalConstants;

/// <summary>
/// Email generation methods
/// 
/// Check EmailSettings in secrets for configuration options
/// Need to go to google account and setup app password for this to work
/// 
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class EmailService : BaseService, EmailServiceI
    {

        private readonly EmailSettings _settings;

        public EmailService(IServiceProvider serviceProvider,
            IOptions<EmailSettings> settings)
            : base(serviceProvider)
        {
            _settings = settings.Value;
        }


        //public string RenderTemplate(ResetRequestEmail request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Template()))
        //        throw new ArgumentException("Template cannot be empty");

        //    var template = Scriban.Template.Parse(request.TemplateHtml());

        //    if (template.HasErrors)
        //        throw new Exception("Template parsing failed");

        //    var result = template.Render(request.Data, member => member.Name);

        //    return result;
        //}


        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


    }
}
