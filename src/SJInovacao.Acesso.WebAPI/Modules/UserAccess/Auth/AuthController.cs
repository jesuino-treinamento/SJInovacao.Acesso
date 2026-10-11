using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature;
using System.Security.Claims;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginRequest> _logger;

        public AuthController(IMediator mediator, IMapper mapper, ILogger<LoginRequest> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

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
            return Ok(new ApiResponse { Success = true, Message = "Logout realizado com sucesso" });
        }
    }
}