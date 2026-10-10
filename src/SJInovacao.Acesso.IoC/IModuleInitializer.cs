using Microsoft.AspNetCore.Builder;

namespace SJInovacao.Acesso.IoC
{
    public interface IModuleInitializer
    {
        void Initialize(WebApplicationBuilder builder);

        Task InitializeAsync(WebApplicationBuilder builder) => Task.CompletedTask;  
    }
}
