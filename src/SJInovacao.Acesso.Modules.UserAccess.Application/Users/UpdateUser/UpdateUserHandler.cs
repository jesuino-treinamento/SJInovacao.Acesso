using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UpdateUserHandler> _logger;

        public UpdateUserHandler(IUserRepository userRepository,
            IAddressRepository addressRepository, IMapper mapper,
            IPasswordHasher passwordHasher, ILogger<UpdateUserHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {           
                // Validação
                var validator = new UpdateUserCommandValidator();
                var validationResult = await validator.ValidateAsync(command, cancellationToken);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validação falhou para usuário {UserId}: {Errors}",
                        command.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new ValidationException(validationResult.Errors);
                }

                var existingUser = await _userRepository.GetByIdUserAddressesPhonesAsync(command.Id, cancellationToken);
                if (existingUser == null)
                {
                    _logger.LogError("Usuário {UserId} não encontrado", command.Id);
                    throw new DomainException($"User with ID {command.Id} not found for update");
                }

                // Verificação de email/username
                if ((existingUser.Email != command.Email) || (existingUser.Username != command.Username))
                {
                    var exists = await _userRepository.ExistsWithEmailOrUsernameAsync(
                        command.Email, command.Username, cancellationToken);

                    if (exists)
                    {
                        _logger.LogWarning("Email {Email} ou Username {Username} já em uso", command.Email, command.Username);
                        throw new DomainException($"Email {command.Email} or username {command.Username} already in use");
                    }
                }

                // Atualiza dados principais
                existingUser.Username = command.Username;
                existingUser.Email = command.Email;
                existingUser.Role = command.Role;
                existingUser.Status = command.Status;

                if (!string.IsNullOrEmpty(command.Password))
                {
                    existingUser.Password = _passwordHasher.HashPassword(command.Password);
                    _logger.LogDebug("Senha do usuário {UserId} atualizada", command.Id);
                }

                existingUser.UpdateName(command.Name.FirstName, command.Name.LastName);

                // Telefones
                existingUser.ClearPhones();
                foreach (var phoneDto in command.Phones)
                {
                    existingUser.AddPhone(new Phone(phoneDto.Number, phoneDto.Type, existingUser));
                }

                // Endereços
                existingUser.ClearAddresses();
                foreach (var addressDto in command.Addresses)
                {
                    var geolocation = new Geolocation(addressDto.Geolocation.Lat, addressDto.Geolocation.Long);
                    existingUser.AddAddress(new Address(
                        addressDto.Street,
                        addressDto.Number,
                        addressDto.Neighborhood,
                        addressDto.City,
                        addressDto.State,
                        addressDto.ZipCode,
                        addressDto.Id,
                        geolocation
                    ));
            }

                var updatedUser = await _userRepository.UpdateUserGroupsPermissions(existingUser, cancellationToken);
                return _mapper.Map<UserResult>(updatedUser);
            
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar usuário {UserId}", command.Id);
                throw;
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Erro de domínio ao atualizar usuário {UserId}", command.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado ao atualizar usuário {UserId}", command.Id);
                throw;
            }
        }
    }
}
