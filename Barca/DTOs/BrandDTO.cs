using Barca.Entities;

namespace Barca.DTOs
{
    public class BrandDTO
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Thumbnail { get; set; }

        public string Headquarters { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public List<ProductDTO>? Products { get; set; }
    }
}
