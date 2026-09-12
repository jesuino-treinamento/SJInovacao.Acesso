using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email).SetValidator(new EmailValidator());

            RuleFor(user => user.Username)
                .NotEmpty()
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Username cannot be longer than 100 characters.");

            RuleFor(user => user.Password).SetValidator(new PasswordValidator());

            RuleFor(user => user.Name)
                .SetValidator(new NameValidator());

            RuleForEach(user => user.Phones)
                .SetValidator(new PhoneValidator());

            RuleFor(user => user.Document)
                .SetValidator(new DocumentValidator());

            RuleForEach(user => user.Addresses)
                .SetValidator(new AddressValidator());

            RuleFor(user => user.Status)
                .NotEqual(StatusTypes.Unknown)
                .WithMessage("User status cannot be Unknown.");

            RuleFor(user => user.Role)
                .NotEqual(UserRole.None)
                .WithMessage("User role cannot be None.");
        }
    }
}
