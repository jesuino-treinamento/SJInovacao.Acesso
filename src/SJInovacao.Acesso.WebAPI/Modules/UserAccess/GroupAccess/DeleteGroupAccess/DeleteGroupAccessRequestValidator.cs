using FluentValidation;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.DeleteGroupAccess
{
    public class DeleteGroupAccessRequestValidator : AbstractValidator<DeleteGroupAccessRequest>
    {
        public DeleteGroupAccessRequestValidator()
        {
            //RuleFor(x => x.GroupAccessId).NotEmpty();
            //RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.GroupAccessId).NotNull();
        }
    }
}
