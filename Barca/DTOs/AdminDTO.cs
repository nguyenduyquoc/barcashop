using Barca.Entities;

namespace Barca.DTOs
{
    public class AdminDTO
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public string Role { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
