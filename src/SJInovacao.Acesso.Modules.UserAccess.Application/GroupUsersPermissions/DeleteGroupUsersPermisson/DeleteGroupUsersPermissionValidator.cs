using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission
{
    public class DeleteGroupUsersPermissionValidator : AbstractValidator<DeleteGroupUsersPermissionCommand>
    {
        public DeleteGroupUsersPermissionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required.");
            RuleFor(x => x.GroupAccessId).NotEmpty().WithMessage("Group Access id is required.");
        }
    }
}
