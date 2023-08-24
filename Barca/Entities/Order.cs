using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int Status { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal? Subtotal { get; set; }

    public decimal? DeliverFee { get; set; }

    public decimal? GrandTotal { get; set; }

    public string? Note { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool PaymentStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
