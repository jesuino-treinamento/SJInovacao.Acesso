using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.DeleteUser
{
    public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
    }
}
