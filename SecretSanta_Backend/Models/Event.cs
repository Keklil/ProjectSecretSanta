using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.Models
{
    public partial class Event
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateOnly? Endofregistration { get; set; }
        public DateOnly? Endofevent { get; set; }
    }
}
