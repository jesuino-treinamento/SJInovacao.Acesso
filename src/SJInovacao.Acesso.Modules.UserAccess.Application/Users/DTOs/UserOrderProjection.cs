namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs
{
    public class UserOrderProjection
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty; 
        public string City { get; set; } = string.Empty;
        public int AddressType { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

}
