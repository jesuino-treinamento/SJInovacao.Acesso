using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.CreateGroupPermission
{
    public class CreateGroupPermissionRequestValidator : AbstractValidator<CreateGroupPermissionRequest>
    {
        public CreateGroupPermissionRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
            RuleForEach(x => x.PermissionIds).NotEmpty();

        }
    }
}
