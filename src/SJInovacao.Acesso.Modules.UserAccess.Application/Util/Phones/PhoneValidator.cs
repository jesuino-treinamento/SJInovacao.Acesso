using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones
{
    public class PhoneValidator : AbstractValidator<PhoneCommand>
    {
        public PhoneValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number must be in international format (E.164). Example: +5511987654321");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid phone type.");
        }
    }
}
