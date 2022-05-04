namespace SecretSanta_Backend.ModelsDTO
{
    public class UserEventView
    {
        public string? Description { get; set; }
        public DateTime EndRegistration { get; set; }
        public DateTime? EndEvent { get; set; }
        public int? SumPrice { get; set; }
        public string? Preference { get; set; }
        public DateTime? SendDay { get; set; }
        public int UsersCount { get; set; } 
    }
}
