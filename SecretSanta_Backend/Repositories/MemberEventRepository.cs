using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;

namespace SecretSanta_Backend.Repositories
{
    public class MemberEventRepository : RepositoryBase<MemberEvent>, IMemberEventRepository
    {
        public MemberEventRepository(ApplicationContext context) : base(context)
        {

        }
    }
}
