using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission
{
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
    {
        // private readonly DefaultContext _context;
        private readonly IPermissionRepository _permissionRepository;

        private readonly IMapper _mapper;

        public UpdatePermissionHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<PermissionDto> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            //var permission = new Permission { Name = request.Name,  Description = request.Description };
            //_context.Permissions.Add(permission);
            //await _context.SaveChangesAsync(cancellationToken);
            //return _mapper.Map<PermissionResult>(permission);

            var validator = new UpdatePermissionValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var exist = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (exist == null)
            {
                throw new DomainException($"Permission with ID {request.Id} not found for update");
            }

            exist.Name = request.Name;
            exist.Description = request.Description;
            exist.IsActive = request.IsActive;

            var permission = _mapper.Map<Permission>(exist);
            var createdPermission = await _permissionRepository.UpdateAsync(permission, cancellationToken);
            var result = _mapper.Map<PermissionDto>(createdPermission);
            return result;
        }
    }
}
