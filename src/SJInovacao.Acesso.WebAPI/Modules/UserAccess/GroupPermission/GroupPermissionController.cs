using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreaterPermissionGroup;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus;
using SJInovacao.Acesso.WebAPI.Common;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermission
{
    [Route("api/groupPermission")]
    [ApiController]
    public class GroupPermissionController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupPermissionController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }        

        [HttpPost("{groupId:guid}/{permissionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGroupPermissionAccess(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            //if (request == null) return BadRequest();
            try
            {
                var command = new CreaterPermissionGroupCommand
                {
                    GroupId = groupId,
                    PermissionId = permissionId
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

        // PUT: api/groupAccess/groupPermissions/{groupId}/{permissionId}/status
        [HttpPut("{groupId:guid}/{permissionId:guid}/{status:bool}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> UpdateStatus(Guid userId, Guid permissionId, [FromBody] UpdateUserPermissionStatusResult request, CancellationToken ct)
        public async Task<IActionResult> UpdateStatus(Guid groupId, Guid permissionId, bool status, CancellationToken ct)
        {
            //if (request == null) return BadRequest();
            try
            {
                var command = new UpdateGroupPermissionStatusCommand
                {
                    GroupId = groupId,
                    PermissionId = permissionId,
                    IsActive = status
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

        // DELETE: api/groupAccess/groupPermissions/{groupId}/{permissionId}
        [HttpDelete("{groupId:guid}/{permissionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            try
            {
                var command = new RemoveGroupPermissionCommand
                {
                    GroupId = groupId,
                    PermissionId = permissionId
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
    }
}
