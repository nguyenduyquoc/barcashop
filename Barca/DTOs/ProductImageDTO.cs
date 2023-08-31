using Barca.Entities;

namespace Barca.DTOs
{
    public class ProductImageDTO
    {
        public int? Id { get; set; }

        public int? ProductId { get; set; }

        public int? MatchKindId { get; set; }

        public string? ImagePath { get; set; }

        public string? MatchKindName { get; set; }

        public string? ProductName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
