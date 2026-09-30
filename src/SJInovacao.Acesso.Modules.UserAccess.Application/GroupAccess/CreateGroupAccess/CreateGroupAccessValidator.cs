using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessValidator : AbstractValidator<CreateGroupAccessCommand>
    {
        public CreateGroupAccessValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Group name is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Group description is required.");
        }
    }
}
