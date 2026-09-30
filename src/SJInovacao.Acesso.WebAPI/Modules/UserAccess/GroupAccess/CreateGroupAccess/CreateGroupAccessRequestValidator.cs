using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessRequestValidator : AbstractValidator<CreateGroupAccessRequest>
    {
        public CreateGroupAccessRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
            RuleForEach(x => x.PermissionIds).NotEmpty();

        }
    }
}
