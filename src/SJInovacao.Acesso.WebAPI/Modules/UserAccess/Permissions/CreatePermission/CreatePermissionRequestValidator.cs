using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission
{
    public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
    {
        public CreatePermissionRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(205);
        }
    }
}
