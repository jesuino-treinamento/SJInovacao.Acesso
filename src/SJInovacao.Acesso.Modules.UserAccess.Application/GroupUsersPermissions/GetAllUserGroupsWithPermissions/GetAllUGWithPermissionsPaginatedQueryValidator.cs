using FluentValidation;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUGWithPermissionsPaginatedQueryValidator
        : AbstractValidator<GetAllUGWithPermissionsPaginatedQuery>
    {
        private static readonly HashSet<string> _validFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "username",
            "userid",
            "groupname",
            "name",       // alias de groupname
            "groupid"
        };

        private static readonly HashSet<string> _validDirections = new(StringComparer.OrdinalIgnoreCase)
        {
            "asc",
            "desc"
        };

        private const int MaxSizeValue = 100;
        private const int MaxMaxResults = 1000;
        private const int MaxPermissionNameLength = 100;

        public GetAllUGWithPermissionsPaginatedQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page deve ser maior que zero.");

            RuleFor(x => x.Size)
                .InclusiveBetween(1, MaxSizeValue)
                .WithMessage($"Size deve estar entre 1 e {MaxSizeValue}.");

            RuleFor(x => x.Order)
                .Must(BeValidOrderExpression)
                .When(x => !string.IsNullOrWhiteSpace(x.Order))
                .WithMessage(
                    $"Order inválido. Campos válidos: {string.Join(", ", _validFields)}. " +
                    "Direções: asc, desc. Formato: 'campo direção, campo2 direção2'.");

            RuleFor(x => x.MaxResults)
                .GreaterThan(0)
                .When(x => x.MaxResults.HasValue)
                .WithMessage("MaxResults deve ser maior que zero.");

            RuleFor(x => x.MaxResults)
                .LessThanOrEqualTo(MaxMaxResults)
                .When(x => x.MaxResults.HasValue)
                .WithMessage($"MaxResults não pode exceder {MaxMaxResults}.");

            RuleFor(x => x.PermissionNameFilter)
                .MaximumLength(MaxPermissionNameLength)
                .When(x => !string.IsNullOrEmpty(x.PermissionNameFilter))
                .WithMessage($"PermissionNameFilter não pode exceder {MaxPermissionNameLength} caracteres.");
        }

        private static bool BeValidOrderExpression(string order)
        {
            if (string.IsNullOrWhiteSpace(order)) return true;

            foreach (var clause in order.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = clause.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return false;

                // Campo
                if (!_validFields.Contains(parts[0]))
                    return false;

                // Direção (opcional)
                if (parts.Length > 1 && !_validDirections.Contains(parts[1]))
                    return false;

                // No máximo 2 partes (campo + direção)
                if (parts.Length > 2)
                    return false;
            }

            return true;
        }
    }
}