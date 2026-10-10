using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Phone : BaseEntity, IDeactivatable
    {
        public string Number { get; private set; } = string.Empty;      
        public PhoneType Type { get; private set; }               
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!; 
        public bool IsActive { get; private set; } = true;         

        // Construtor privado para EF Core
        public Phone() { }

        /// <summary>
        /// Construtor para criar um novo telefone
        /// </summary>
        /// <param name="number">Número do telefone (não pode ser vazio)</param>
        /// <param name="type">Tipo do telefone</param>
        /// <param name="user">Pessoa associada (opcional)</param>
        /// <param name="branch">Filial associada (opcional)</param>
        /// <exception cref="ArgumentException">Lançada quando número é inválido ou não tem associação</exception>
        public Phone(string number, PhoneType type, Person person)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Número de telefone inválido.");

            if (person == null)
                throw new ArgumentNullException(nameof(person), "Usuário associado é obrigatório.");

            Number = number;
            Type = type;
            Person = person;
            PersonId = person.Id;
            IsActive = true;
        }

        /// <summary>
        /// Desativa o telefone (marca como inativo)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o telefone (marca como ativo)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}