using Microsoft.EntityFrameworkCore;
using Quartz;
using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Repositories;
using SecretSanta_Backend.Services;

namespace SecretSanta_Backend.Jobs
{
    public class EventNotificationSender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var repository = new RepositoryWrapper();
            var mailService = new MailService();
            var events = await repository.Event.FindAll().ToListAsync();
            foreach (var @event in events)
                if (@event.EndRegistration == DateTime.Today)
                    if (@event.Reshuffle == false)
                    {
                        await mailService.sendEmailsWithDesignatedRecipient(@event.Id);
                    }            
        }
    }
}
