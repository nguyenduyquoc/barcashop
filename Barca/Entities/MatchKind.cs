using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class MatchKind
{
    public int Id { get; set; }

    public string? MatchKindName { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
