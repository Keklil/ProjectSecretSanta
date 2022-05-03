using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.Models
{
    public partial class Event
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime EndRegistration { get; set; }
        public DateTime? EndEvent { get; set; }
        public int? SumPrice { get; set; }
        public bool? SendFriends { get; set; }
        public bool? Tracking { get; set; }
        public bool? Reshuffle { get; set; }
    }
}