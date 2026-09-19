using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Validation;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.UpdateUser
{
    public class UpdateUserRequestValidator : AbstractValidator<UserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required")
                .NotEqual(Guid.Empty).WithMessage("User ID cannot be empty");

            //RuleFor(x => x.Username)
            //    .NotEmpty().WithMessage("Username is required")
            //    .Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

            RuleFor(x => x.Password)
                .SetValidator(new PasswordValidator())
                .When(x => !string.IsNullOrEmpty(x.Password));

            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email is required")
            //    .SetValidator(new EmailValidator());

            RuleFor(x => x.Status)
                .NotEqual(StatusTypes.Unknown).WithMessage("Status cannot be Unknown");

            RuleFor(x => x.Role)
                .NotEqual(UserRole.None).WithMessage("Role cannot be None");

            RuleFor(x => x.Name).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Name.FirstName).NotEmpty().MaximumLength(50);
                RuleFor(x => x.Name.LastName).NotEmpty().MaximumLength(50);
            });

            RuleForEach(x => x.Phones).ChildRules(phones =>
            {
                phones.RuleFor(p => p.Number)
                    .Matches(@"^\+?[1-9]\d{1,14}$")
                    .WithMessage("Phone number must be in international format (E.164). Example: +5511987654321");

                phones.RuleFor(p => p.Type)
                    .IsInEnum().WithMessage("Invalid phone type");
            });

            RuleForEach(x => x.Addresses).ChildRules(address =>
            {
                address.RuleFor(a => a.City).NotEmpty().MaximumLength(100);
                address.RuleFor(a => a.Street).NotEmpty().MaximumLength(200);
                address.RuleFor(a => a.Number).NotEmpty();
                address.RuleFor(a => a.ZipCode).NotEmpty().MaximumLength(20);

                address.RuleFor(a => a.Geolocation).NotNull().DependentRules(() =>
                {
                    address.RuleFor(a => a.Geolocation.Lat)
                        .NotEmpty().WithMessage("Latitude is required");

                    address.RuleFor(a => a.Geolocation.Long)
                        .NotEmpty().WithMessage("Longitude is required");
                });
            });
        }
    }
}
