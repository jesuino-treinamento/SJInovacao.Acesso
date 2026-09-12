using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Validation
{
    public class GeolocationValidator : AbstractValidator<Geolocation>
    {
        public GeolocationValidator()
        {
            RuleFor(geo => geo.Lat)
                .NotEmpty().WithMessage("Latitude is required")
                .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Invalid latitude format");

            RuleFor(geo => geo.Long)
                .NotEmpty().WithMessage("Longitude is required")
                .Matches(@"^-?\d{1,3}(\.\d{1,6})?$").WithMessage("Invalid longitude format");
        }
    }
}
