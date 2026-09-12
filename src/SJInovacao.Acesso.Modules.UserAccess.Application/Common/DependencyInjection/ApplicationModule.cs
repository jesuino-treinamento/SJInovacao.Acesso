using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SJInovacao.Acesso.Common.Security.Authentication;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Common.DependencyInjection
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationModule(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();//PermissionRequirement : IAuthorizationRequirement
                                                                                            //services.AddScoped<IAuthorizationHandler, HasPermissionAuthorizationHandler>();

            services.AddScoped<IAuthorizationRequirement, PermissionRequirement>();//PermissionPolicyProvider : IAuthorizationPolicyProvider
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
