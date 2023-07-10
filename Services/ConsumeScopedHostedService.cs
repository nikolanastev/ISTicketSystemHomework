using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interface;

namespace Services;

public class ConsumeScopedHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ConsumeScopedHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await DoWork();
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IBackgroundEmailSender>();
        await scopedProcessingService.DoWork();
    }
}