using Barca.Entities;

namespace Barca.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string? Avatar { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Admin? Admin { get; set; }

        public List<UserAddressDTO>? UserAddresses { get; set; }
    }
}
