using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class UserDiscount
{
    public int? UserId { get; set; }

    public int? DiscountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual DiscountCode? Discount { get; set; }

    public virtual User? User { get; set; }
}
