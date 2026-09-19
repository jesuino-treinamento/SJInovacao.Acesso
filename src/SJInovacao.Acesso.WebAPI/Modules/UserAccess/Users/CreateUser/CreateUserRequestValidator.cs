using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Validation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser
{
    public class CreateUserRequestValidator : AbstractValidator<UserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3).WithMessage("O nome de usuário deve ter pelo menos 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome de usuário não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Password)
                .SetValidator(new PasswordValidator())
                .When(x => !string.IsNullOrEmpty(x.Password));

            RuleFor(x => x.Email)
                .SetValidator(new EmailValidator());

            RuleFor(x => x.Status)
                .NotEqual(StatusTypes.Unknown)
                .WithMessage("O status do usuário não pode ser Desconhecido.");

            RuleFor(x => x.Role)
                .NotEqual(UserRole.None)
                .WithMessage("O Role do usuário não pode ser Nenhuma.");

            RuleFor(x => x.Name).NotNull().DependentRules(() =>
            {
                RuleFor(name => name.Name.FirstName)
                    .NotEmpty().WithMessage("O primeiro nome é obrigatório.")
                    .MinimumLength(3).WithMessage("O primeiro nome deve ter pelo menos 3 caracteres.")
                    .MaximumLength(100).WithMessage("O primeiro nome não pode exceder 100 caracteres.");

                RuleFor(name => name.Name.LastName)
                    .NotEmpty().WithMessage("O último nome é obrigatório.")
                    .MinimumLength(3).WithMessage("O último nome deve ter pelo menos 3 caracteres.")
                    .MaximumLength(100).WithMessage("O último nome não pode exceder 100 caracteres");
            });

            RuleForEach(x => x.Phones).ChildRules(phones =>
            {
                phones.RuleFor(x => x.Number)
                    .NotEmpty().WithMessage("Número de telefone obrigatório.")
                    .Matches(@"^\+?[1-9]\d{1,14}$")
                    .WithMessage("O número de telefone deve estar no formato internacional (E.164). Exemplo: +5511987654321");

                phones.RuleFor(x => x.Type)
                    .IsInEnum().WithMessage("Tipo de telefone inválido.");
            });

            RuleForEach(x => x.Addresses).ChildRules(address =>
            {
                address.RuleFor(addr => addr.City)
                   .NotEmpty().WithMessage("Cidade não pode ser vazio ou nulo.")
                   .MaximumLength(100).WithMessage("Cidade não pode exceder 100 caracteres");

                address.RuleFor(addr => addr.Street)
                    .NotEmpty().WithMessage("Street is required")
                    .MaximumLength(200).WithMessage("Street não pode exceder 200 caracteres");

                address.RuleFor(addr => addr.State)
                    .NotEmpty().WithMessage("Estado não pode ser vazio ou nulo.")
                    .MaximumLength(100).WithMessage("Estado não pode exceder 100 caracteres");

                address.RuleFor(addr => addr.Number)
                    .NotEmpty().WithMessage("Address number is required");

                address.RuleFor(addr => addr.ZipCode)
                    .NotEmpty().WithMessage("CEP inválido.")
                    .Matches(@"^\d{5}-?\d{3}$").WithMessage("Formato de código postal inválido");

                address.RuleFor(a => a.Geolocation).NotNull().DependentRules(() =>
                {
                    address.RuleFor(a => a.Geolocation.Lat)
                        .NotEmpty().WithMessage("Latitude obrigatória.")
                        .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Formato de latitude inválido.");

                    address.RuleFor(a => a.Geolocation.Long)
                        .NotEmpty().WithMessage("Longitude obrigatória.")
                        .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Formato de longitude inválido.");
                });
            });
        }
    }
}
