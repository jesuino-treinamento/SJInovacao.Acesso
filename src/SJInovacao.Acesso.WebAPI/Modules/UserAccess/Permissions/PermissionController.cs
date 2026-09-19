using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.UpdatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions
{
    // PermissionController.cs
    [Route("api/permissions")]
    [ApiController]
    public class PermissionController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PermissionController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<PermissionResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new CreatePermissionRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<CreatePermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(string.Empty, new ApiResponseWithData<PermissionResponse>
                {
                    Success = true,
                    Message = "Permission created successfully",
                    Data = _mapper.Map<PermissionResponse>(response)
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
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<PermissionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseWithData<PermissionResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.Id = id;
                var validator = new UpdatePermissionRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<UpdatePermissionCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(string.Empty, new ApiResponseWithData<PermissionResponse>
                {
                    Success = true,
                    Message = "Permission updated successfully",
                    Data = _mapper.Map<PermissionResponse>(response)
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
        [Group("group.all")]
        [Permission("user.view", "user.update")] // 🔒 exige permissão
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<List<PermissionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new ListPermissionsQuery(), cancellationToken);

                // Mapear a lista de PermissionResult para lista de PermissionResponse
                var response = _mapper.Map<List<PermissionResponse>>(result);

                return Ok(response, "Permission list successfully");
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
