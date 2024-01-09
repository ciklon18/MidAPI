using Quartz;

namespace MisAPI.Quartz;

public class JobWrapper : IJob, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly IJob? _job;

    public JobWrapper(IServiceProvider serviceProvider, Type jobType)
    {
        _serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _job = ActivatorUtilities.CreateInstance(_serviceScope.ServiceProvider, jobType) as IJob;
        // создает инстанс класса типа jobType, который реализует IJob и передает в него все зависимости
        
        // _job = _serviceScope.ServiceProvider.GetService(jobType) as IJob;
        // то же самое, что и выше, однако не передает зависимости в конструктор
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _job?.Execute(context) ?? throw new InvalidOperationException();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}