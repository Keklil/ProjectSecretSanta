namespace SecretSanta_Backend.ModelsDTO
{
    public class EventsByMember
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime EndRegistration { get; set; }
        public DateTime? EndEvent { get; set; }
        public int? SumPrice { get; set; }
        public bool? Tracking { get; set; }
        public bool? Attend { get; set; }
    }
}
