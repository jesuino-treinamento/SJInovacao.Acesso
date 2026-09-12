using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.AddUserPermission
{
    public class RemoveGroupUserCommandValidator : AbstractValidator<RemoveGroupUserCommand>
    {
        public RemoveGroupUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.GroupUsersId).NotEmpty();
        }
    }
}
