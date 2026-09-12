using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Validation
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(addr => addr.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(addr => addr.Street)
                .NotEmpty().WithMessage("Street is required")
                .MaximumLength(200).WithMessage("Street cannot exceed 200 characters");

            RuleFor(addr => addr.Number)
                .NotEmpty().WithMessage("Address number is required");

            RuleFor(addr => addr.ZipCode)
                .NotEmpty().WithMessage("Zip code is required")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("Invalid zip code format");

            RuleFor(addr => addr.Geolocation)
                .NotNull().WithMessage("Geolocation is required")
                .SetValidator(new GeolocationValidator());
        }
    }
}
