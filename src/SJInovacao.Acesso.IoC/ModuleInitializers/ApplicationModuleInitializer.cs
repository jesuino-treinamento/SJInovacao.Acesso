using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SJInovacao.Acesso.Common.Auditing;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Common.Security.Authentication;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using SJInovacao.Acesso.Common.Security.Context;
using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Application;
using SJInovacao.Acesso.Modules.UserAccess.Application.Contracts;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using System.Reflection;

namespace SJInovacao.Acesso.IoC.ModuleInitializers
{
    public class ApplicationModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, HybridPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, GroupAuthorizationHandler>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IUserContext, UserContext>();

            builder.Services.AddScoped<IAuditService, AuditService>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IUserAccessModule, UserAccessModule>();

            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<DefaultContext>((sp, options) =>
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SJInovacao.Acesso.Database")
                );
            });

            var webApiAssembly = Assembly.GetEntryAssembly()
                ?? Assembly.Load("SJInovacao.Acesso.WebAPI");

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(UserAccessModule).Assembly);      
                cfg.AddMaps(typeof(Permission).Assembly);             
                cfg.AddMaps(typeof(DefaultContext).Assembly);         
                cfg.AddMaps(webApiAssembly);                          
            });

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(UserAccessModule).Assembly,
                    typeof(DefaultContext).Assembly,
                    webApiAssembly
                );
            });

            builder.Services.AddValidatorsFromAssemblies(new[]
            {
                typeof(UserAccessModule).Assembly,
                typeof(DefaultContext).Assembly,
                webApiAssembly
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            return Task.CompletedTask;
        }
    }
}