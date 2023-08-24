using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? SizeId { get; set; }

    public int? MatchKindId { get; set; }

    public decimal RootPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public int Quantity { get; set; }

    public virtual MatchKind? MatchKind { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }
}
