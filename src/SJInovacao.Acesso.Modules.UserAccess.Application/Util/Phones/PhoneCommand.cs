using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using MediatR;
using SJInovacao.Acesso.Common.Validation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones
{
    public class PhoneCommand : IRequest<PhoneDto>
    {
        public Guid Id { get; set; }
        public Guid PhoneId { get; set; }
        public string Number { get; set; } = string.Empty;
        public PhoneType Type { get; set; }

        public ValidationResultDetail Validate()
        {
            var validator = new PhoneValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }
}
