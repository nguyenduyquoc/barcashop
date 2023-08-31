using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class Admin
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
