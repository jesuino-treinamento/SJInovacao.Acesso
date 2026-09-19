using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature;
using System.Security.Claims;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth
{
    /// <summary>
    /// Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginRequest> _logger;

        /// <summary>
        /// Initializes a new instance of AuthController
        /// </summary>
        /// <param name="mediator">The mediator instance</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public AuthController(IMediator mediator, IMapper mapper, ILogger<LoginRequest> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user with their credentials
        /// </summary>
        /// <param name="request">The authentication request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication token if successful</returns>

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Tentativa de login: {Email}", request.Email);
                var validator = new AuthenticateUserRequestValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var command = _mapper.Map<AuthenticateUserCommand>(request);
                var response = await _mediator.Send(command, cancellationToken);

                return Ok(response, "User authenticated successfully");
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
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            { 
            var command = new RefreshTokenCommand { RefreshToken = request.RefreshToken };
            var response = await _mediator.Send(command);

            //var response = new AuthResponse
            //{
            //    AccessToken = result.AccessToken,
            //    RefreshToken = result.RefreshToken,
            //    ExpiresIn = 28800,
            //    User = new UserInfoDto
            //    {
            //        Id = result.Id,
            //        Name = result.Name,
            //        Email = result.Email,
            //        Username = result.Name,
            //        Role = result.Role,
            //        Permissions = result.Permissions
            //    }
            //};
            //return Ok(new ApiResponse<AuthResponse> { Success = true, Message = "Token renovado com sucesso", Data = response });
            return Ok(response, "Token renovado com sucesso");
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

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var command = new RevokeRefreshTokenCommand { UserId = Guid.Parse(userId) };
                await _mediator.Send(command);
            }
            //return Ok(ApiResponse, "Token renovado com sucesso");
            return Ok(new ApiResponse { Success = true, Message = "Logout realizado com sucesso" });
        }
    }
}