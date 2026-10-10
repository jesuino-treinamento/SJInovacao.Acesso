using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.DeleteGroupAccess;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.UpdateGroupAccess;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetGroupPermissions;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.CreateGroupAccess;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.UpdateGroupAccess;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupsAccess
{
    [Route("api/groupAccess")]
    [ApiController]
    public class GroupAccessController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupAccessController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseWithData<GroupAccessResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupAccess([FromBody] CreateGroupAccessRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new CreateGroupAccessRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<CreateGroupPermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(string.Empty, new ApiResponseWithData<GroupAccessResponse>
                {
                    Success = true,
                    Message = "Group access created successfully",
                    Data = _mapper.Map<GroupAccessResponse>(response)
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
        

        [HttpPut("{groupId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateGroupStatus(Guid groupId, [FromBody] UpdateGroupAccessRequest request, CancellationToken ct)
        {
            try
            {
                var command = new UpdateGroupAccessCommand
                {
                    GroupId = groupId,
                    Name = request.Name,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    PermissionIds = request.PermissionIds
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

        [HttpDelete("{groupId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(Guid groupId, CancellationToken ct)
        {
            try
            {
                var command = new DeleteGroupAccessCommand
                {
                    GroupId = groupId
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
