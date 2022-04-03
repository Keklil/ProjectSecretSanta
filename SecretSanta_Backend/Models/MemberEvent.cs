using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.Models
{
    public partial class MemberEvent
    {
        public Guid MemberId { get; set; }
        public Guid EventId { get; set; }

        public virtual Event Event { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
    }
}
