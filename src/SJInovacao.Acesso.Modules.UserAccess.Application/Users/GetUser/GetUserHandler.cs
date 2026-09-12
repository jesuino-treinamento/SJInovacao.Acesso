using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserCommand, UserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserHandler(
             IUserRepository userRepository,
             IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResult> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var validator = new GetUserValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"Usuário com ID {request.Id} não encontrado");

            //user.Password = "";//_passwordHasher.HashPassword(user.Password);

            var _user = _mapper.Map<UserResult>(user);

            return _user;
        }
    }
}
