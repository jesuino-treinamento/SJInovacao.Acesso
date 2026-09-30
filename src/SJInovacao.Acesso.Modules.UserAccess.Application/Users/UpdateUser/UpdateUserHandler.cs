using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly DefaultContext _context;
        private readonly ILogger<UpdateUserHandler> _logger;

        public UpdateUserHandler(IUserRepository userRepository,
            IAddressRepository addressRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher,DefaultContext defaultContext, ILogger<UpdateUserHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _context = defaultContext;
            _logger = logger;
        }

        //    public async Task<UserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        //    {
        //        var validator = new UpdateUserCommandValidator();
        //        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        //        if (!validationResult.IsValid)
        //            throw new ValidationException(validationResult.Errors);

        //        var existingUser = await _userRepository.GetByIdAsync(command.Id, cancellationToken);

        //        if (existingUser == null)
        //        {
        //            throw new DomainException($"User with ID {command.Id} not found for update");
        //        }

        //        if ((existingUser.Email != command.Email) || (existingUser.Username != command.Username))
        //        {
        //            var isEmailOrUsernameToken = await _userRepository.ExistsWithEmailOrUsernameAsync(
        //               command.Email,
        //               command.Username,
        //               cancellationToken);

        //            if (isEmailOrUsernameToken)
        //            {
        //                throw new DomainException($"Email {command.Email} or username {command.Username} already in use by another user");
        //            }
        //        }

        //        existingUser.Username = command.Username;
        //        existingUser.Email = command.Email;
        //        existingUser.Role = command.Role;
        //        existingUser.Status = command.Status;

        //        if (!string.IsNullOrEmpty(command.Password))
        //        {
        //            existingUser.Password = _passwordHasher.HashPassword(command.Password);
        //        }

        //        existingUser.UpdateName(command.Name.FirstName, command.Name.LastName);
        //        existingUser.ClearPhones();
        //        // Atualiza os telefones
        //        foreach (var phoneDto in command.Phones)
        //        {                
        //            var userPhone = new Phone(phoneDto.Number, phoneDto.Type, existingUser);
        //            existingUser.AddPhone(userPhone);
        //        }

        //        existingUser.ClearAddresses();
        //        // Atualiza os endereços
        //        foreach (var addressDto in command.Addresses)
        //        {
        //            var geolocation = new Geolocation(addressDto.Geolocation.Lat, addressDto.Geolocation.Long);

        //            var userAddress = new Address(
        //                addressDto.Street,
        //                addressDto.Number,
        //                addressDto.Neighborhood,
        //                addressDto.City,
        //                addressDto.State,
        //                addressDto.ZipCode,
        //                addressDto.Id,
        //                geolocation
        //            );

        //            existingUser.AddAddress(userAddress);
        //        }

        //        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);
        //        return _mapper.Map<UserResult>(updatedUser);
        //    }

        //}

        public async Task<UserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogInformation("Iniciando atualização do usuário {UserId}", command.Id);

                // Atualiza UsersGroupsPermissions
                var entities = await _context.UsersGroupsPermissions
                    .Where(ugp => ugp.UserId == command.Id)
                    .ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    entity.IsActive = command.Status == StatusTypes.Active;
                    entity.UpdatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Atualizando UserPermissions para usuário {UserId}", command.Id);

                // Atualiza UserPermissions
                var usersPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == command.Id)
                    .ToListAsync(cancellationToken);

                foreach (var userPermission in usersPermissions)
                {
                    userPermission.IsActive = command.Status == StatusTypes.Active;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                }

                // Atualiza UserGroup
                var usersGroups = await _context.UserGroup
                    .Where(up => up.UserId == command.Id)
                    .ToListAsync(cancellationToken);

                foreach (var usersGroup in usersGroups)
                {
                    usersGroup.IsActive = command.Status == StatusTypes.Active;
                    usersGroup.UpdatedAt = DateTime.UtcNow;
                }

                // Validação
                var validator = new UpdateUserCommandValidator();
                var validationResult = await validator.ValidateAsync(command, cancellationToken);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validação falhou para usuário {UserId}: {Errors}",
                        command.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new ValidationException(validationResult.Errors);
                }

                var existingUser = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
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

                var updatedUser = _context.Users.Update(existingUser);// _userRepository.UpdateAsync(existingUser, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Usuário {UserId} atualizado com sucesso", command.Id);

                return _mapper.Map<UserResult>(updatedUser.Entity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar usuário {UserId}", command.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Erro de domínio ao atualizar usuário {UserId}", command.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado ao atualizar usuário {UserId}", command.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

}
