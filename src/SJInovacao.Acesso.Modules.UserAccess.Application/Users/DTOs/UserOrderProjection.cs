using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs
{
    public class UserOrderProjection
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Campos de endereço para ordenação
        public string Street { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty; 
        public string City { get; set; } = string.Empty;
        public int AddressType { get; set; } // supondo que exista um campo tipo no Address

        public DateTime CreatedAt { get; set; }
    }

}
