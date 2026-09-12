using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission
{
    public class CreatePermissionValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
        }
    }
}
