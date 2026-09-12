using FluentValidation;

namespace SJInovacao.Acesso.Common.Validation
{
    public static class Validator
    {
        /// <summary>
        /// Localiza e executa todos os validators do tipo <see cref="IValidator{T}"/> carregados no AppDomain.
        /// Retorna a lista de erros (convertidos para <see cref="ValidationErrorDetail"/>).
        /// </summary>
        public static async Task<IEnumerable<ValidationErrorDetail>> ValidateAsync<T>(T instance)
        {
            var validatorInterface = typeof(IValidator<T>);

            var validatorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => !t.IsAbstract && validatorInterface.IsAssignableFrom(t))
                .ToList();

            var errors = new List<ValidationErrorDetail>();

            foreach (var type in validatorTypes)
            {
                if (Activator.CreateInstance(type) is IValidator<T> validator)
                {
                    var result = await validator.ValidateAsync(new FluentValidation.ValidationContext<T>(instance));
                    if (!result.IsValid)
                        errors.AddRange(result.Errors.Select(e => (ValidationErrorDetail)e));
                }
            }

            return errors;
        }
    }
}