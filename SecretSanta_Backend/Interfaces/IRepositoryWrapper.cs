namespace SecretSanta_Backend.Interfaces
{
    public interface IRepositoryWrapper
    {
        IEventRepository Event { get; }
        IMemberRepository Member { get; }
        void Save();
    }
}
