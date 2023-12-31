﻿using Barca.Entities;

namespace Barca.DTOs
{
    public class MatchKindDTO
    {
        public int? Id { get; set; }

        public string? MatchKindName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public List<ProductImageDTO>? ProductImages { get; set; }
    }
}
