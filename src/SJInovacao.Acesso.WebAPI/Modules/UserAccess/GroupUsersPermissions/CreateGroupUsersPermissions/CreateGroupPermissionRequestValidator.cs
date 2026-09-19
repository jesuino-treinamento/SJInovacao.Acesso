using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.CreateGroupUsersPermissions
{
    public class CreateGroupUsersPermissionRequestValidator : AbstractValidator<CreateGroupUsersPermissionsRequest>
    {
        public CreateGroupUsersPermissionRequestValidator()
        {
            RuleFor(x => x.GroupAccessId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();

        }
    }
}
