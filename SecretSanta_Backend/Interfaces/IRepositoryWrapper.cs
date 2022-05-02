namespace SecretSanta_Backend.Interfaces
{
    public interface IRepositoryWrapper
    {
        IEventRepository Event { get; }
        IMemberRepository Member { get; }
        IMemberEventRepository MemberEvent { get; }
        IAddressRepository Address { get; }
        void Save();
        Task SaveAsync();
    }
}
