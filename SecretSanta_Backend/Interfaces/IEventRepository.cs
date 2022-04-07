using SecretSanta_Backend.Models;

namespace SecretSanta_Backend.Interfaces
{
    public interface IEventRepository: IRepositoryBase<Event>
    {
        void CreateEvent(Event @event);
        void DeleteEvent(Event @event);
        void UpdateEvent(Event @event);

    }
}
