using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses
{
    public class AddressValidator : AbstractValidator<AddressCommand>
    {
        public AddressValidator()
        {
            RuleFor(addr => addr.City)
                 .NotEmpty().WithMessage("Cidade não pode ser vazio ou nulo.")
                 .MaximumLength(100).WithMessage("Cidade não pode exceder 100 caracteres");

            RuleFor(addr => addr.Street)
                .NotEmpty().WithMessage("Street is required")
                .MaximumLength(200).WithMessage("Street não pode exceder 200 caracteres");

            RuleFor(addr => addr.State)
                .NotEmpty().WithMessage("Estado não pode ser vazio ou nulo.")
                .MaximumLength(100).WithMessage("Estado não pode exceder 100 caracteres");

            RuleFor(addr => addr.Number)
                .NotEmpty().WithMessage("Address number is required");

            RuleFor(addr => addr.ZipCode)
                .NotEmpty().WithMessage("CEP inválido.")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("Formato de código postal inválido");           

            RuleFor(geo => geo.Geolocation.Lat)
                .NotEmpty().WithMessage("Latitude obrigatória.")
                .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Formato de latitude inválido.");

            RuleFor(geo => geo.Geolocation.Long)
                .NotEmpty().WithMessage("Longitude obrigatória.")
                .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Formato de longitude inválido.");          
        }
    }
}
