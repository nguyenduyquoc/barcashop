namespace Barca.DTOs
{
    public class ListProductImages
    {
        public List<ProductImageDTO>? ProductImages { get; set; }

        public int? TotalPages { get; set; }

        public int TotalItems { get; set; }
    }
}
