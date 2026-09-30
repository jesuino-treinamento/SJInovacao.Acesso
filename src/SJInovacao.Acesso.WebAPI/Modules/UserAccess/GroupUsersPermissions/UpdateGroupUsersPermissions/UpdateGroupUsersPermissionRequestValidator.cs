using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.UpdateGroupUsersPermissions
{
    public class UpdateGroupUsersPermissionRequestValidator : AbstractValidator<UpdateGroupUsersPermissionsRequest>
    {
        public UpdateGroupUsersPermissionRequestValidator()
        {
            //RuleFor(x => x.GroupAccessId).NotEmpty();
            //RuleFor(x => x.UserId).NotEmpty();
            //RuleFor(x => x.PermissionIsActive).NotNull();
        }
    }
}
