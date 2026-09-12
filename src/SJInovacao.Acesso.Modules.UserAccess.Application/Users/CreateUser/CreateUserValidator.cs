using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.Name.FirstName).NotEmpty();
            RuleFor(x => x.Name.LastName).NotEmpty();

            RuleFor(x => x.Document.Number).NotEmpty();
            RuleFor(x => x.Document.PersonType).IsInEnum();

            RuleForEach(cmd => cmd.Addresses)
                .SetValidator(new Util.Addresses.AddressValidator());

            RuleForEach(cmd => cmd.Phones)
                .NotNull().WithMessage("Phone is required")
                .SetValidator(new Util.Phones.PhoneValidator());
        }
    }
}
