using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GetUser
{
    public class GetUserValidator : AbstractValidator<GetUserCommand>
    {
        public GetUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID do usuário é obrigatório");
        }
    }
}
