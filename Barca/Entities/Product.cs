using System;
using System.Collections.Generic;

namespace Barca.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? YearOfManufacture { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public int ClubId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual FootballClub Club { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
