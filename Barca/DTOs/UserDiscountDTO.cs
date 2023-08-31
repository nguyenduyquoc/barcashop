using Barca.Entities;

namespace Barca.DTOs
{
    public class UserDiscountDTO
    {
        public int? UserId { get; set; }

        public int? DiscountId { get; set; }

        public virtual DiscountCode? Discount { get; set; }

        public virtual User? User { get; set; }
    }
}
