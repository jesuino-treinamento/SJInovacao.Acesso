namespace SJInovacao.Acesso.WebAPI.Common.Response
{
    public class PersonResponse
    {
        public NameResponse Name { get; set; } = new();
        public DocumentResponse Document { get; set; } = new();
        public List<AddressResponse> Addresses { get; set; } = new();
        public List<PhoneResponse> Phones { get; set; } = new();
    }
}
