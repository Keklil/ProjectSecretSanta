using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;

namespace SecretSanta_Backend.Repositories
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(ApplicationContext context) : base(context)
        {

        }

        public void CreateEvent(Event @event) => Create(@event);
        public void UpdateEvent(Event @event) => Update(@event);
        public void DeleteEvent(Event @event) => Delete(@event);

    }
}
