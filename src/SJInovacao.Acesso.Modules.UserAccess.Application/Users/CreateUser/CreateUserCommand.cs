using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using MediatR;

public class CreateUserCommand : IRequest<CreateUserResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public StatusTypes Status { get; set; }
    public UserRole Role { get; set; }

    public NameCommand Name { get; set; } = new();
    public DocumentCommand Document { get; set; } = new();
    public List<AddressCommand> Addresses { get; set; } = new();
    public List<PhoneCommand> Phones { get; set; } = new();    

    // Força o uso de dados fake (ignora os campos opcionais)
    public bool UseFakeData { get; set; } = false;
}

