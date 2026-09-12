namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util
{
    public class AddressDto
    {
        public Guid Id { get; set; }
       // public Guid AddressId { get; set; }

        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public GeolocationResult? Geolocation { get; set; } = new();
    }

    public class GeolocationResult
    {
        public string Lat { get; set; } = string.Empty;
        public string @Long { get; set; } = string.Empty;
    }
}