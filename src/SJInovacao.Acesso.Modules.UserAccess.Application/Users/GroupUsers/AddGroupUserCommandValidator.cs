using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GroupUsers
{
    public class AddGroupUserCommandValidator : AbstractValidator<AddGroupUserCommand>
    {
        public AddGroupUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.GroupUserId).NotEmpty();
        }
    }
}
