using Microsoft.AspNetCore.Builder;

namespace SJInovacao.Acesso.IoC
{
    public interface IModuleInitializer
    {
        // Inicialização síncrona
        void Initialize(WebApplicationBuilder builder);

        // Inicialização assíncrona (opcional)
        Task InitializeAsync(WebApplicationBuilder builder) => Task.CompletedTask;  // implementação padrão
    }
}
