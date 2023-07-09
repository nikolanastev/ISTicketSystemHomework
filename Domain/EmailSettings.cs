namespace Domain;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public int SmtpServerPort { get; set; }
    public bool EnableSsl { get; set; }
    public string EmailDisplayName { get; set; }
    public string SendersName { get; set; }

    public EmailSettings(string smtpServer, string smtpUsername, string smtpPassword, int smtpServerPort)
    {
        SmtpServer = smtpServer;
        SmtpUsername = smtpUsername;
        SmtpPassword = smtpPassword;
        SmtpServerPort = smtpServerPort;
    }

    public EmailSettings()
    {
        
    }
}