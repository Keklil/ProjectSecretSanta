using System;
using System.Collections.Generic;

namespace SecretSanta_Backend.ModelsDTO
{
    public partial class MemberDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
