using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using SJInovacao.Acesso.Modules.UserAccess.Application.Contracts;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

namespace SJInovacao.Acesso.IoC.ModuleInitializers
{
    public class InfrastructureModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPersonRepository, PersonRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IPhoneRepository, PhoneRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IGroupPermissionRepository, GroupPermissionRepository>();
            builder.Services.AddScoped<IGroupsPermissionsRepository, GroupsPermissionsRepository>();
            builder.Services.AddScoped<IGroupPermissionUserRepository, GroupPermissionUserRepository>();
            builder.Services.AddScoped<IUsersGroupsPermissionsRepository, UsersGroupsPermissionsRepository>();
            builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            builder.Services.AddScoped<IUserAccessModule, UserAccessModule>();//HybridPolicyProvider
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, GroupAuthorizationHandler>();
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            Initialize(builder);
            return Task.CompletedTask;
        }
    }
}
