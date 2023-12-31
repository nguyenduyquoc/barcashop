﻿using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class MatchKind
{
    public int Id { get; set; }

    public string? MatchKindName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
