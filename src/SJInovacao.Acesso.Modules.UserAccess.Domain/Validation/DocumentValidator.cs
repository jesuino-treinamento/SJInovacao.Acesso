using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Validation
{
    public class DocumentValidator : AbstractValidator<Document>
    {
        public DocumentValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Document number is required.");

            RuleFor(x => x.PersonType)
                .IsInEnum().WithMessage("Invalid person type.");

            RuleFor(x => x)
                .Must(x => IsValidDocument(x.Number, x.PersonType))
                .WithMessage("Invalid CPF or CNPJ.");
        }

        private bool IsValidDocument(string number, PersonType type)
        {
            try
            {
                var _ = new Document(number, type);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}