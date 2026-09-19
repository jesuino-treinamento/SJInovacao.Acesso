using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.GetUser
{
    public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
