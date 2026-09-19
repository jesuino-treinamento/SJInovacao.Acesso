using FluentValidation;
namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.UpdatePermission
{
    public class UpdatePermissionRequestValidator : AbstractValidator<UpdatePermissionRequest>
    {
        public UpdatePermissionRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(205);
            RuleFor(x => x.IsActive).NotNull();
        }
    }
}

