using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class ProductVariant
{
    public int? ProductId { get; set; }

    public int? SizeId { get; set; }

    public int? MatchKindId { get; set; }

    public decimal RootPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public int Quantity { get; set; }

    public int? QuantitySold { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Thumbnail { get; set; }

    public virtual MatchKind? MatchKind { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }
}
