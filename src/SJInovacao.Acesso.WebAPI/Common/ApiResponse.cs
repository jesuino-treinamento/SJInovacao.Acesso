using SJInovacao.Acesso.Common.Validation;

namespace SJInovacao.Acesso.WebAPI.Common
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<ValidationErrorDetail> Errors { get; set; } = [];
    }
}
