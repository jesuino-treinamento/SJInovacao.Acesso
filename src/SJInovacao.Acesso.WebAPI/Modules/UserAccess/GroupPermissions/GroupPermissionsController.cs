using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Features.GroupPermissions.CreateGroupPermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.CreateGroupPermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsers
{
    [Route("api/userAccess/groupAccess")]
    [ApiController]
    public class GroupPermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupPermissionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseWithData<GroupPermissionResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupAccess([FromBody] CreateGroupPermissionRequest request, CancellationToken cancellationToken)
        {
            //var command = _mapper.Map<CreateGroupUserCommand>(request);
            //var result = await _sender.Send(command, cancellationToken);
            //return Created(string.Empty, new ApiResponseWithData<GroupUserResponse>(_mapper.Map<GroupUserResponse>(result)));
            try
            {
                var validator = new CreateGroupPermissionRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<CreateGroupPermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(string.Empty, new ApiResponseWithData<GroupPermissionResponse>
                {
                    Success = true,
                    Message = "Group access created successfully",
                    Data = _mapper.Map<GroupPermissionResponse>(response)
                });
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

        // GET: api/groupAccess/groupPermissions/{groupId}
        [HttpGet("{groupId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid groupId, CancellationToken ct)
        {
            try
            {
                var query = new GetGroupPermissionsQuery { GroupId = groupId };
                var result = await _mediator.Send(query, ct);
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

        // GET: api/groupAccess/groupPermissions
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                var query = new GetAllGroupsWithPermissionsQuery();
                var result = await _mediator.Send(query, ct);
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
