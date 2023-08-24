using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class Size
{
    public int Id { get; set; }

    public string? SizeName { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
