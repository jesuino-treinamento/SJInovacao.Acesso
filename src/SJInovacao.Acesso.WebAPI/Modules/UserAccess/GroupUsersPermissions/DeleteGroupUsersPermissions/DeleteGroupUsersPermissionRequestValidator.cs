using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.DeleteGroupUsersPermissions
{
    public class DeleteGroupUsersPermissionRequestValidator : AbstractValidator<DeleteGroupUsersPermissionsRequest>
    {
        public DeleteGroupUsersPermissionRequestValidator()
        {
            //RuleFor(x => x.GroupAccessId).NotEmpty();
            //RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.UserIsActive).NotNull();
        }
    }
}
