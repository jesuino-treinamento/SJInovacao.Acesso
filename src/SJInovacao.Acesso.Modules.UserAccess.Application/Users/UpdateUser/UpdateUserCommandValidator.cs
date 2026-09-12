using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Validation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(user => user.Email).SetValidator(new EmailValidator());
            RuleFor(user => user.Username).NotEmpty().Length(3, 50);
            RuleFor(user => user.Password).SetValidator(new PasswordValidator());
            RuleFor(user => user.Status).NotEqual(StatusTypes.Unknown);
            RuleFor(user => user.Role).NotEqual(UserRole.None);

            RuleFor(cmd => cmd.Document)
               .NotNull().WithMessage("Document is required")
               .SetValidator(new DocumentValidator());
            RuleFor(cmd => cmd.Name)
                .NotNull().WithMessage("Name is required")
                .SetValidator(new NameValidator());

            RuleForEach(cmd => cmd.Addresses)
                .SetValidator(new Util.Addresses.AddressValidator());

            RuleForEach(cmd => cmd.Phones)
                .NotNull().WithMessage("Phone is required")
                .SetValidator(new Util.Phones.PhoneValidator());
        }
    }
}
