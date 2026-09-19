namespace SJInovacao.Acesso.WebAPI.Common.Request
{
    public class AddressRequest
    {
        public Guid Id { get; internal set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public GeolocationRequest Geolocation { get; set; } = new();
    }
}