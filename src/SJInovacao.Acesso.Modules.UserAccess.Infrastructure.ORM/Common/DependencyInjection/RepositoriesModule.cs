using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Common.DependencyInjection
{
    public static class RepositoriesModule
    {
        public static IServiceCollection AddRepositoriesModule(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IPhoneRepository, PhoneRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IGroupPermissionRepository, GroupPermissionRepository>();

            return services;
        }
    }
}
