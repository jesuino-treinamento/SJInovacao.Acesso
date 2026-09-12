using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionValidator : AbstractValidator<CreateGroupPermissionCommand>
    {
        public CreateGroupPermissionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Group name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Group description is required.");
        }
    }
}
