using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.ModelsDTO
{
    public partial class MemberEventDTO
    {
        public Guid MemberId { get; set; }
        public Guid EventId { get; set; }
        public bool? MemberAttend { get; set; }
        public string? DeliveryService { get; set; }
        public string? TrackNumber { get; set; }
        public string? Preference { get; set; }
        public Guid? Recipient { get; set; }
        public DateTime? Sendday { get; set; }

        public virtual EventDTO Event { get; set; } = null!;
        public virtual MemberDTO Member { get; set; } = null!;
    }
}
