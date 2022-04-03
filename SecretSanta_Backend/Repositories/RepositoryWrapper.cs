using SecretSanta_Backend.Interfaces;
using SecretSanta_Backend.Models;

namespace SecretSanta_Backend.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationContext context;
        private IMemberRepository member;
        private IEventRepository @event;

        public IMemberRepository Member
        {
            get
            {
                if (member == null)
                {
                    member = new MemberRepository(context);
                }
                return member;
            }
        }

        public IEventRepository Event
        {
            get
            {
                if (@event == null)
                {
                    @event = new EventRepository(context);
                }
                return @event;
            }
        }

        public RepositoryWrapper(ApplicationContext context)
        {
            this.context = context;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
