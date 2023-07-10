namespace Services.Interface;

public interface IBackgroundEmailSender
{
    Task DoWork();
}