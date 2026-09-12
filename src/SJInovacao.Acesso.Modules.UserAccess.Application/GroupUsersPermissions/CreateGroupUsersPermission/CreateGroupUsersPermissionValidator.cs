using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission
{
    public class CreateGroupUsersPermissionValidator : AbstractValidator<CreateGroupUsersPermissionCommand>
    {
        public CreateGroupUsersPermissionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required.");
            RuleFor(x => x.GroupAccessId).NotEmpty().WithMessage("Group Access id is required.");
        }
    }
}
