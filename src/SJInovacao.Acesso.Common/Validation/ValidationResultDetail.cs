using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SJInovacao.Acesso.Common.Validation
{
    public class ValidationResultDetail
    {
        public bool IsValid { get; set; }
        public IEnumerable<ValidationErrorDetail> Errors { get; set; } = Enumerable.Empty<ValidationErrorDetail>();

        public ValidationResultDetail()
        {
        }

        public ValidationResultDetail(ValidationResult validationResult)
        {
            IsValid = validationResult.IsValid;
            Errors = validationResult.Errors.Select(o => (ValidationErrorDetail)o);
        }
    }
}