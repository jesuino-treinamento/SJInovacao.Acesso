using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPhoneRepository _phoneRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserHandler(IUserRepository userRepository,
            IAddressRepository addressRepository,
            IPhoneRepository phoneRepository, IPersonRepository personRepository, IPermissionRepository permissionRepository,
            IMapper mapper, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _phoneRepository = phoneRepository;
            _personRepository = personRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.ExistsWithEmailOrUsernameAsync(command.Email, command.Username, cancellationToken);
            if (existingUser)
                throw new InvalidOperationException($"User with email {command.Email} or UserName {command.Username} already exists");

            var document = new Document(command.Document.Number, command.Document.PersonType);
            var name = new Name(command.Name.FirstName, command.Name.LastName);

            var person = new Person(name, document);


            var user = new User(
                command.Username,
                command.Email,
                _passwordHasher.HashPassword(command.Password),
                command.Role,
                name,
                document
            );

            // 4. Criar telefone (US01: sempre terá pelo menos um telefone)
            Phone phone;
            if (command.UseFakeData)
            {
                // Telefone fake
                //var fakeNumber = new Phone();// FakeDataHelper.GetFakePhoneNumber();
                phone = new Phone(); //new Phone(fakeNumber, PhoneType.Celular, user);
                user.AddPhone(phone);
            }
            else
            {
                // Inicializa listas
                foreach (var phoneDto in command.Phones)
                {
                    phone = await _phoneRepository.GetByIdAsync(phoneDto.PhoneId, cancellationToken);// ?? new (); 

                    if (phone == null)
                    {
                        var userphone = new Phone(phoneDto.Number, phoneDto.Type, person);
                        user.AddPhone(userphone);
                    }
                }

            }


            // 5. Criar endereço (US01: sempre terá pelo menos um endereço)
            Address address;
            if (command.UseFakeData)
            {
                // Endereço fake
                //var fakeStreet = FakeDataHelper.GetFakeStreet();
                //var fakeNumber = FakeDataHelper.GetFakeNumber();
                //var fakeNeighborhood = FakeDataHelper.GetFakeNeighborhood();
                //var fakeCity = FakeDataHelper.GetFakeCity();
                //var fakeState = FakeDataHelper.GetFakeState();
                //var fakeZipCode = FakeDataHelper.GetFakeZipCode();
                //var (lat, lng) = FakeDataHelper.GetFakeGeolocation();
                var geolocation = new Geolocation();// (lat, lng);

                address = new Address();
                    //fakeStreet, fakeNumber, fakeNeighborhood,
                    //fakeCity, fakeState, fakeZipCode,
                    //user.Id, geolocation);

                user.AddAddress(address);
            }
            else
            {
                foreach (var addressDto in command.Addresses)
                {
                    address = await _addressRepository.GetByIdAsync(addressDto.Id, cancellationToken);// ?? new();
                    if (address == null)
                    {
                        var geolocation = new Geolocation(addressDto.Geolocation.Lat, addressDto.Geolocation.Long);

                        var userAddress = new Address(
                            addressDto.Street,
                            addressDto.Number,
                            addressDto.Neighborhood,
                            addressDto.City,
                            addressDto.State,
                            addressDto.ZipCode,
                            person.Id,
                            geolocation
                        );

                        user.AddAddress(userAddress);
                    }
                }
            }

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

            
            var result = _mapper.Map<CreateUserResult>(createdUser);
            return result;
        }

    }
}