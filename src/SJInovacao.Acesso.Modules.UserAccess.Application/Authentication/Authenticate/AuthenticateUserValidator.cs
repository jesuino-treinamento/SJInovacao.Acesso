using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    public class AuthenticateUserValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Login � obrigat�rio")
                .MinimumLength(6).WithMessage("Email deve ter no m�nimo 6 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha � obrigat�ria")
                .MinimumLength(6).WithMessage("Senha deve ter no m�nimo 6 caracteres")
                .MaximumLength(50).WithMessage("Senha deve ter no m�ximo 50 caracteres");
        }
    }
}
