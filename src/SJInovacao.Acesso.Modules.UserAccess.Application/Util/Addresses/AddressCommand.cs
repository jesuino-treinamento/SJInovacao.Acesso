using MediatR;
using SJInovacao.Acesso.Common.Validation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses
{
    public class AddressCommand : IRequest<AddressDto>
    {
        public Guid Id { get; set; }
        //public Guid AddressId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty; // bairro
        public string State { get; set; } = string.Empty;

        public GeolocationCommand Geolocation { get; set; } = new();

        public ValidationResultDetail Validate()
        {
            var validator = new AddressValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }        
    }

    public class GeolocationCommand
    {
        public string Lat { get; set; } = string.Empty;
        public string @Long { get; set; } = string.Empty;
    }
}
