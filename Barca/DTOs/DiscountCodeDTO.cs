namespace Barca.DTOs
{
    public class DiscountCodeDTO
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public int? MPercent { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
