using Barca.Entities;

namespace Barca.DTOs
{
    public class SizeDTO
    {
        public int? Id { get; set; }

        public string? SizeName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
