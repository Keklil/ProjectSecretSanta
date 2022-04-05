using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.Models
{
    public partial class MemberEvent
    {
        public Guid MemberId { get; set; }
        public Guid EventId { get; set; }
        public bool? MemberAttend { get; set; }
        public string? DeliveryService { get; set; }
        public string? TrackNumber { get; set; }
        public string? Preference { get; set; }
        public Guid? Recipient { get; set; }
        public DateTime? Sendday { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
    }
}
