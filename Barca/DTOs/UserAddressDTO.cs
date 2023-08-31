using Barca.Entities;

namespace Barca.DTOs
{
    public class UserAddressDTO
    {
        public int? Id { get; set; }

        public string Address { get; set; }

        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }
    }
}
