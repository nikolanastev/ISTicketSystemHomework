namespace Domain.DomainModels;

public class EmailMessage : BaseEntity
{
    public string? MailTo { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public bool Status { get; set; }
}