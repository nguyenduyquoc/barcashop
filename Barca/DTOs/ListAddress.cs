namespace Barca.DTOs
{
    public class ListAddress
    {
        public List<UserAddressDTO>? UserAddresses { get; set; }

        public int? TotalPages { get; set; }

        public int TotalItems { get; set; }
    }
}
