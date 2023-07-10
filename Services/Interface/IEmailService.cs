using Domain.DomainModels;

namespace Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync(List<EmailMessage> allMails);
}