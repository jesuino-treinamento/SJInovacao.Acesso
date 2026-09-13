using SJInovacao.Acesso.IoC.ModuleInitializers;
using Microsoft.AspNetCore.Builder;

namespace SJInovacao.Acesso.IoC
{
    public static class DependencyResolver
    {
        public static async Task RegisterDependenciesAsync(this WebApplicationBuilder builder)
        {
            var applicationModule = new ApplicationModuleInitializer();
            var infrastructureModule = new InfrastructureModuleInitializer();
            var webApiModule = new WebApiModuleInitializer();

            applicationModule.Initialize(builder);
            await applicationModule.InitializeAsync(builder);

            infrastructureModule.Initialize(builder);
            await infrastructureModule.InitializeAsync(builder);

            webApiModule.Initialize(builder);
            await webApiModule.InitializeAsync(builder);
        }
    }
}
