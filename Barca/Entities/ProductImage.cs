using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class ProductImage
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? MatchKindId { get; set; }

    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual MatchKind? MatchKind { get; set; }

    public virtual Product? Product { get; set; }
}
