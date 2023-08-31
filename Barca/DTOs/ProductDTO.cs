using Barca.Entities;

namespace Barca.DTOs
{
    public class ProductDTO
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public DateTime? YearOfManufacture { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? BrandId { get; set; }

        public string? BrandName { get; set; }

        public int ClubId { get; set; }

        public string? FootballClubName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public List<OrderProductDTO>? OrderProducts { get; set; }

        public List<ProductImageDTO>? ProductImages { get; set; }
    }
}
