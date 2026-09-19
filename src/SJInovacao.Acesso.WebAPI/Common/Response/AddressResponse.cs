namespace SJInovacao.Acesso.WebAPI.Common.Response
{
    public class AddressResponse
    {
        public Guid Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public GeolocationResponse Geolocation { get; set; } = new();
    }
}