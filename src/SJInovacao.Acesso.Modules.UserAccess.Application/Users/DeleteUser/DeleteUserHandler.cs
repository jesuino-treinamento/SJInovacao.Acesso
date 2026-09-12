using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var validator = new DeleteUserValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var success = await _userRepository.DesativarAsync(request.Id, cancellationToken);
            if (!success)
                throw new KeyNotFoundException($"User with ID {request.Id} not found");

            return new DeleteUserResponse { Success = true };
        }
    }
}
