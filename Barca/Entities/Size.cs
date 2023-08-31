using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class Size
{
    public int Id { get; set; }

    public string? SizeName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
