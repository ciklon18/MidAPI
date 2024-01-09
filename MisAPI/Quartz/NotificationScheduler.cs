using Quartz;
using Quartz.Impl;

namespace MisAPI.Quartz;

public static class NotificationScheduler
{

    public static async Task Start(IServiceProvider serviceProvider)
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        scheduler.JobFactory = serviceProvider.GetService<JobFactory>()
                               ?? throw new InvalidOperationException();
        await scheduler.Start();

        
        var jobDetail = CreateJobDetail();
        var trigger = CreateTrigger();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }
    
    
    
    private static IJobDetail CreateJobDetail() 
    {
        const string jobKey = nameof(NotificationBackgroundJob);
        var jobDetail = JobBuilder.Create<NotificationBackgroundJob>()
            .WithIdentity(jobKey)
            .Build();
        return jobDetail;
    }
    
    private static ITrigger CreateTrigger()
    {
        const string triggerKey = nameof(NotificationBackgroundJob);
        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();
        return trigger;
    }
}