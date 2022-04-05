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
        public int? Sumprice { get; set; }
        public bool? Sandfriends { get; set; }
        public bool? Tracking { get; set; }
    }
}
