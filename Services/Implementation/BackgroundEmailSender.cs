using Domain.DomainModels;
using Repository.Interface;
using Services.Interface;

namespace Services.Implementation;

public class BackgroundEmailSender : IBackgroundEmailSender
{
    
    private readonly IEmailService _emailService;
    private readonly IRepository<EmailMessage> _mailRepository;

    public BackgroundEmailSender(IEmailService emailService, IRepository<EmailMessage> mailRepository)
    {
        _emailService = emailService;
        _mailRepository = mailRepository;
    }

    public async Task DoWork()
    {
        await _emailService.SendEmailAsync(_mailRepository.GetAll().Where(z => !z.Status).ToList());
    }
}