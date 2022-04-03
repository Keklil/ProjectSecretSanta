using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;

namespace SecretSanta_Backend.Repositories
{
    public class MemberRepository : RepositoryBase<Member>, IMemberRepository
    {
        public MemberRepository(ApplicationContext context) : base(context)
        {

        }
    }
}
