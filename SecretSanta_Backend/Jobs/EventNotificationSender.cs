using Microsoft.EntityFrameworkCore;
using Quartz;
using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Services;

namespace SecretSanta_Backend.Jobs
{
    public class EventNotificationSender : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            var repository = RepositoryTransfer.GetRepository();
            var events = await repository.Event.FindAll().ToListAsync();
            var mailService = new MailService();
            foreach (var @event in events)
            {
                if (@event.EndRegistration == DateTime.Today)
                {
                    await mailService.sendEmailsWithDesignatedRecipient(@event.Id);
                }              
            }
        }
    }
}
