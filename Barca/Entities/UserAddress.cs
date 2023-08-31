using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class UserAddress
{
    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
