namespace Barca.DTOs
{
    public class ListProduct
    {
        public List<ProductDTO>? Products { get; set; }

        public int? TotalPages { get; set; }

        public int TotalItems { get; set; }

    }
}
