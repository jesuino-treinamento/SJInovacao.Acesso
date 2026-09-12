namespace SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects
{
    public class Geolocation
    {
        public string Lat { get; set; } = string.Empty;
        public string @Long { get; set; } = string.Empty;

        public Geolocation() 
        {
        }

        public Geolocation(string lat, string @long)
        {
            Lat = lat;
            Long = @long;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Lat))
                throw new ArgumentException("Latitude cannot be empty");

            if (string.IsNullOrWhiteSpace(Long))
                throw new ArgumentException("Longitude cannot be empty");
        }
    }
}
