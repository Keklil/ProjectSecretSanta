using Quartz;
using Quartz.Impl;

namespace SecretSanta_Backend.Jobs
{
    public class EventNotificationScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            

            IJobDetail job = JobBuilder.Create<EventNotificationSender>().Build();

            ITrigger trigger = TriggerBuilder.Create() 
                .WithIdentity("trigger1", "group1")   
                .StartNow()
                //.StartAt(DateBuilder.TodayAt(12, 0, 0))
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())                  
                .Build();                             

            await scheduler.ScheduleJob(job, trigger);       
        }
    }
}