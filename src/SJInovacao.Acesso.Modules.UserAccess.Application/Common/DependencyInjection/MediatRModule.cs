using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Common.DependencyInjection
{
    public static class MediatRModule
    {
        public static IServiceCollection AddMediatRModule(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}
