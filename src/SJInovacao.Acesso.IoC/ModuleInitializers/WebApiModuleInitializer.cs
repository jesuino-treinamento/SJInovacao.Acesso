using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SJInovacao.Acesso.IoC.ModuleInitializers
{
    public class WebApiModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            Initialize(builder);
            return Task.CompletedTask;
        }
    }
}
