using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Common.Security.Authentication;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DeleteUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.GetUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.ListUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.DeleteUser;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.GetUser;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.UpdateUser;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users
{
    /// <summary>
    /// Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UsersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        //[Authorize(Policy = "CanManageUsers")] // 🔒 exige permissão
        [ProducesResponseType(typeof(ApiResponseWithData<UserResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new CreateUserRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<CreateUserCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Created(command.Role.ToString(), new ApiResponseWithData<UserResponse>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = _mapper.Map<UserResponse>(response)
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

       // [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<UserResult>>> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string order = "username asc, email desc",
        CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetAllUsersQuery
                {
                    Page = page,
                    Size = size,
                    Order = order
                };

                var result = await _mediator.Send(query, cancellationToken);

                return OkPaginated(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        //[Authorize(Roles = "Admin")]
        [Permission(SegurityPermissions.UserDelete)] // 🔒 exige permissão
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var request = new DeleteUserRequest { Id = id };
                var validator = new DeleteUserRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<DeleteUserCommand>(request.Id);
                await _mediator.Send(command, cancellationToken);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "User deleted successfully"
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetUserRequest { Id = id };
                var validator = new GetUserRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<GetUserCommand>(request.Id);
                var response = await _mediator.Send(command, cancellationToken);

                if (response == null)
                    return NotFound("User not found");

                var result = _mapper.Map<UserResponse>(response);
                return Ok(result, "Usuário recuperado com sucesso");
                //return Ok(ApiResponseWithData<UserResponse>.SuccessResponse(result, "Usuário recuperado com sucesso"));
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


        //[Authorize(Roles = "Admin , Manager, Customer")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.Id = id;

                var validator = new UpdateUserRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Validation errors occurred",
                        Errors = validationResult.Errors.Select(e => new ValidationErrorDetail { Error = e.ErrorMessage })
                    });

                var command = _mapper.Map<UpdateUserCommand>(request);
                var result = await _mediator.Send(command, cancellationToken);
                var response = _mapper.Map<UserResponse>(result);

                return Ok(new ApiResponseWithData<UserResponse>
                {
                    Success = true,
                    Message = "Sale updated successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }
    }
}
