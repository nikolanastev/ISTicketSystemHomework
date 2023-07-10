using System.Net.Mail;
using Domain;
using Domain.DomainModels;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Services.Interface;

namespace Services.Implementation;

public class EmailService : IEmailService
{

    private readonly EmailSettings _settings;

    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }


    public async Task SendEmailAsync(List<EmailMessage> allMails)
    {
        var messages = new List<MimeMessage>();

        foreach (var item in allMails)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_settings.SendersName, _settings.SmtpUsername),
                Subject = item.Subject
            };
            emailMessage.From.Add(new MailboxAddress(_settings.EmailDisplayName, _settings.SmtpUsername));

            emailMessage.Body = new TextPart(TextFormat.Plain) { Text = item.Content };

            emailMessage.To.Add(new MailboxAddress(item.MailTo, item.MailTo));

            messages.Add(emailMessage);
        }

        try
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            var socketOption = _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpServerPort, socketOption);

            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                await smtp.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
            }

            foreach (var item in messages)
            {
                await smtp.SendAsync(item);
            }

            await smtp.DisconnectAsync(true);
        }
        catch (SmtpException ex)
        {
            throw ex;
        }
        
    }
}