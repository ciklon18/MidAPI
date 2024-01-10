using Quartz;

namespace MisAPI.Quartz.JobFactory;

public class JobWrapper : IJob, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly IJob? _job;
    private readonly ILogger<JobWrapper> _logger;

    public JobWrapper(IServiceProvider serviceProvider, Type jobType)
    {
        _serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _job = ActivatorUtilities.CreateInstance(_serviceScope.ServiceProvider, jobType) as IJob;
        _logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<JobWrapper>>();
        // создает инстанс класса типа jobType, который реализует IJob и передает в него все зависимости
        
        // _job = _serviceScope.ServiceProvider.GetService(jobType) as IJob;
        // то же самое, что и выше, однако не передает зависимости в конструктор
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing job {job}", _job?.GetType().Name);
        return _job?.Execute(context) ?? throw new InvalidOperationException();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}