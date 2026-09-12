using AutoMapper;
using FluentValidation;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateUserHandler(IUserRepository userRepository,
            IAddressRepository addressRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.GetByIdAsync(command.Id, cancellationToken);

            if (existingUser == null)
            {
                throw new DomainException($"User with ID {command.Id} not found for update");
            }

            if ((existingUser.Email != command.Email) || (existingUser.Username != command.Username))
            {
                var isEmailOrUsernameToken = await _userRepository.ExistsWithEmailOrUsernameAsync(
                   command.Email,
                   command.Username,
                   cancellationToken);

                if (isEmailOrUsernameToken)
                {
                    throw new DomainException($"Email {command.Email} or username {command.Username} already in use by another user");
                }
            }

            existingUser.Username = command.Username;
            existingUser.Email = command.Email;
            existingUser.Role = command.Role;
            existingUser.Status = command.Status;

            if (!string.IsNullOrEmpty(command.Password))
            {
                existingUser.Password = _passwordHasher.HashPassword(command.Password);
            }

            existingUser.UpdateName(command.Name.FirstName, command.Name.LastName);
            existingUser.ClearPhones();
            // Atualiza os telefones
            foreach (var phoneDto in command.Phones)
            {                
                var userPhone = new Phone(phoneDto.Number, phoneDto.Type, existingUser);
                existingUser.AddPhone(userPhone);
            }

            existingUser.ClearAddresses();
            // Atualiza os endereços
            foreach (var addressDto in command.Addresses)
            {
                var geolocation = new Geolocation(addressDto.Geolocation.Lat, addressDto.Geolocation.Long);

                var userAddress = new Address(
                    addressDto.Street,
                    addressDto.Number,
                    addressDto.Neighborhood,
                    addressDto.City,
                    addressDto.State,
                    addressDto.ZipCode,
                    addressDto.Id,
                    geolocation
                );

                existingUser.AddAddress(userAddress);
            }

            var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);
            return _mapper.Map<UserResult>(updatedUser);
        }

    }
}
