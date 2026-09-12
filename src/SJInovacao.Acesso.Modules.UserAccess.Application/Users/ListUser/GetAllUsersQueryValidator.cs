using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.ListUser
{
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        private readonly string[] _validProperties = { "username", "email", "phone", "status", "role", "createdAt",  "updatedat", "name", "name.firstname", "name.lastname",
            "address", "address.city", "address.street", "address.zipcode", "geolocation", "geolocation.lat", "geolocation.long" };

        public GetAllUsersQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.Size).InclusiveBetween(1, 100);
            RuleFor(x => x.Order).NotEmpty().Must(BeValidOrderExpression);
        }

        private bool BeValidOrderExpression(string order)
        {
            if (string.IsNullOrWhiteSpace(order)) return false;

            foreach (var clause in order.Split(','))
            {
                var parts = clause.Trim().Split(' ');
                if (!_validProperties.Contains(parts[0].ToLower()))
                    return false;
                if (parts.Length > 1 && !new[] { "asc", "desc" }.Contains(parts[1].ToLower()))
                    return false;
            }
            return true;
        }
    }
}
