using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetAllUsersWithPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetUserPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.RemoveUserPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.UpdateUserPermissionStatus;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissios.CreateUserPermission;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions.CreateUserPermissions;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions
{
    [Route("api/userAccess/userPermissions")]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UserPermissionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // POST: api/userAccess/userPermissions
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserPermissionRequest request, CancellationToken ct)
        {
            try
            {
                if (request == null) return BadRequest();

                var command = new CreateUserPermissionCommand
                {
                    UserId = request.UserId,
                    PermissionId = request.PermissionId
                };

                var result = await _mediator.Send(command, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            
        }

        // PUT: api/userAccess/userPermissions/{userId}/{permissionId}/status
        [HttpPut("{userId:guid}/{permissionId:guid}/{status:bool}")]
        //public async Task<IActionResult> UpdateStatus(Guid userId, Guid permissionId, [FromBody] UpdateUserPermissionStatusResult request, CancellationToken ct)
        public async Task<IActionResult> UpdateStatus(Guid userId, Guid permissionId, bool status, CancellationToken ct)
        {
            //if (request == null) return BadRequest();

            var command = new UpdateUserPermissionStatusCommand
            {
                UserId = userId,
                PermissionId = permissionId,
                IsActive = status
            };

            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        // DELETE: api/userAccess/userPermissions/{userId}/{permissionId}
        [HttpDelete("{userId:guid}/{permissionId:guid}")]
        public async Task<IActionResult> Remove(Guid userId, Guid permissionId, CancellationToken ct)
        {
            var command = new RemoveUserPermissionCommand
            {
                UserId = userId,
                PermissionId = permissionId
            };

            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        // GET: api/userAccess/userPermissions/{userId}
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct)
        {
            var query = new GetUserPermissionsQuery { UserId = userId };
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        // GET: api/userAccess/userPermissions
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var query = new GetAllUsersWithPermissionsQuery();
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }
    }
}
