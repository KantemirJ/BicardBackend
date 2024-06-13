using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Reflection;

namespace BicardBackend.Services;

public interface IEmailService
{
    void Send(string to, string subject, string html);
    Task<string> ReadTemplateFileAsync(string filePath);
}

public class EmailService : IEmailService
{
    private readonly SmtpModel _appSettings;

    public EmailService(IOptions<SmtpModel> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public void Send(string to, string subject, string html)
    {
        // create message
        var email = new MimeMessage();
        var fromAddress = new MailboxAddress("Bicard", "noreply@bicard.com");
        email.From.Add(fromAddress);
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        using var smtp = new SmtpClient();
        smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
    public async Task<string> ReadTemplateFileAsync(string filePath)
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Email template not found: {filePath}");
            }

            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}