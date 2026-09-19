using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissios.CreateUserPermission
{
    public class CreateUserPermissionValidator : AbstractValidator<CreateUserPermissionCommand>
    {
        public CreateUserPermissionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PermissionId).NotEmpty();
            RuleFor(x => x.IsActive).NotEmpty();
        }
    }
}
