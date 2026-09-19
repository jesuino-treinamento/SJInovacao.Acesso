using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Common.DependencyInjection
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationModule(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();//PermissionRequirement : IAuthorizationRequirement
            services.AddScoped<IAuthorizationRequirement, PermissionRequirement>();                                                                             //services.AddScoped<IAuthorizationHandler, HasPermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, GroupPolicyProvider>();
            services.AddScoped<IAuthorizationRequirement, GroupRequirement>();
            //PermissionPolicyProvider : IAuthorizationPolicyProvider
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
