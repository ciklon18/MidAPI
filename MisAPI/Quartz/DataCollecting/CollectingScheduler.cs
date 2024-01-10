using Quartz;
using Quartz.Impl;

namespace MisAPI.Quartz.DataCollecting;


public static class CollectingScheduler
{

    public static async Task Start(IServiceProvider serviceProvider, DateTime? scheduledTime = null)
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        scheduler.JobFactory = serviceProvider.GetService<JobFactory.JobFactory>()
                               ?? throw new InvalidOperationException();
        await scheduler.Start();
        
        var jobDetail = CreateJobDetail();
        var trigger = scheduledTime.HasValue ? CreateScheduledTrigger(scheduledTime.Value) : CreateTrigger();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }
    
    
    
    private static IJobDetail CreateJobDetail() 
    {
        const string jobKey = nameof(DataCollectingJob);
        var jobDetail = JobBuilder.Create<DataCollectingJob>()
            .WithIdentity(jobKey)
            .Build();
        return jobDetail;
    }
    
    private static ITrigger CreateTrigger()
    {
        const string triggerKey = nameof(DataCollectingJob);
        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever())
            .Build();
        return trigger;
    }
    
    private static ITrigger CreateScheduledTrigger(DateTime scheduledDate)
    {
        const string triggerKey = nameof(DataCollectingJob);
        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartAt(new DateTimeOffset(scheduledDate))
            .Build();
        return trigger;
    }
}