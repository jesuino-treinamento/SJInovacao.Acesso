using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission
{
    public class UpdateGroupUsersPermissionValidator : AbstractValidator<UpdateGroupUsersPermissionCommand>
    {
        public UpdateGroupUsersPermissionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required.");
            RuleFor(x => x.GroupAccessId).NotEmpty().WithMessage("Group Access id is required.");
        }
    }
}
