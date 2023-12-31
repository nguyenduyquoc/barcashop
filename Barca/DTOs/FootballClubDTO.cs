﻿using Barca.Entities;

namespace Barca.DTOs
{
    public class FootballClubDTO
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Countries { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public List<ProductDTO>? Products { get; set; }
    }
}
