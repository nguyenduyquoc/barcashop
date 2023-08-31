namespace Barca.DTOs
{
    public class ListUserDiscountCode
    {
        public List<UserDiscountDTO>? UserDiscounts { get; set; }

        public int? TotalPages { get; set; }

        public int TotalItems { get; set; }
    }
}
