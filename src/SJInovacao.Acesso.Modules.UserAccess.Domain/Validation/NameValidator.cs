using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Validation
{
    public class NameValidator : AbstractValidator<Name>
    {
        public NameValidator()
        {
            RuleFor(name => name.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(3).WithMessage("First must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(name => name.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(3).WithMessage("Last must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");
        }
    }
}
