using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Features.GroupPermissions.CreateGroupPermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.CreateGroupPermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.CreateGroupUsersPermissions;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.DeleteGroupUsersPermissions;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.UpdatePermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.DeleteUser;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions
{
    [Route("api/userAccess/GroupUsersPermissions")]
    [ApiController]
    public class GroupUsersPermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupUsersPermissionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateGroupUsersPermissionsResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateGroupUsersPermissionsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new CreateGroupUsersPermissionRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<CreateGroupUsersPermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(string.Empty, new ApiResponseWithData<CreateGroupUsersPermissionsResponse>
                {
                    Success = true,
                    Message = "Group useraccess created successfully",
                    Data = _mapper.Map<CreateGroupUsersPermissionsResponse>(response)
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

        [Authorize]
        [HttpDelete("{UserId:guid}/{GroupAccessId:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<DeleteGroupUsersPermissionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseWithData<DeleteGroupUsersPermissionsResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromRoute] Guid UserId, [FromRoute] Guid GroupAccessId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new DeleteGroupUsersPermissionsRequest { UserId = UserId, GroupAccessId = GroupAccessId };
               
                var validator = new DeleteGroupUsersPermissionRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<DeleteGroupUsersPermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new ApiResponseWithData<DeleteGroupUsersPermissionsResponse>
                {
                    Success = true,
                    Message = "Group useraccess updated successfully",
                    Data = _mapper.Map<DeleteGroupUsersPermissionsResponse>(response)
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
    }
}
