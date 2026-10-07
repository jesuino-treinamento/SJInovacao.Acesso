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
        public string Username { get; set; }
        public string Email { get; set; }

        // Campos de endereço para ordenação
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public int AddressType { get; set; } // supondo que exista um campo tipo no Address

        public DateTime CreatedAt { get; set; }
    }

}
