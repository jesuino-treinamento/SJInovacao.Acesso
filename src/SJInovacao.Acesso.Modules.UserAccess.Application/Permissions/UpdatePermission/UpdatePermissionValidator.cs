using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission
{
    public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionCommand>
    {
        public UpdatePermissionValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
            RuleFor(x => x.IsActive).NotNull();

        }
    }
}

