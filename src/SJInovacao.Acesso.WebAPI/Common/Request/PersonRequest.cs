namespace SJInovacao.Acesso.WebAPI.Common.Request
{
    public class PersonRequest
    {
        public Guid Id { get; set; }
        public NameRequest Name { get; set; } = new();
        public DocumentRequest Document { get; set; } = new();
        public List<AddressRequest> Addresses { get; set; } = new();
        public List<PhoneRequest> Phones { get; set; } = new();
    }
}
