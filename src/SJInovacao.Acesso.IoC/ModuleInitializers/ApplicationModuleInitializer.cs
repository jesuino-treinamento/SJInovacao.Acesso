using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Common.Security.Authentication;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using SJInovacao.Acesso.Common.Security.Context;
using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Application;
using SJInovacao.Acesso.Modules.UserAccess.Application.Contracts;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using System.Reflection;

namespace SJInovacao.Acesso.IoC.ModuleInitializers
{
    public class ApplicationModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            // ===== SECURITY & CONTEXT =====
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, HybridPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, GroupAuthorizationHandler>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IUserContext, UserContext>();

            // ✅ Registra a orquestração de casos de uso (agora vive na Application)
            builder.Services.AddScoped<IUserAccessModule, UserAccessModule>();

            builder.Services.AddAuthorization();

            // ===== DATABASE =====
            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SJInovacao.Acesso.Database")
                )
            );
            
            // ✅ Fallback robusto
            var webApiAssembly = Assembly.GetEntryAssembly()
                ?? Assembly.Load("SJInovacao.Acesso.WebAPI");

            // ===== AUTOMAPPER (tipos marcadores) =====
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(UserAccessModule).Assembly);      // Application
                cfg.AddMaps(typeof(Permission).Assembly);             // Domain
                cfg.AddMaps(typeof(DefaultContext).Assembly);         // Infrastructure
                cfg.AddMaps(webApiAssembly);                          // WebAPI
            });

            // ===== MEDIATR =====
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(UserAccessModule).Assembly,
                    typeof(DefaultContext).Assembly,
                    webApiAssembly
                );
            });

            // ===== FLUENTVALIDATION =====
            builder.Services.AddValidatorsFromAssemblies(new[]
            {
                typeof(UserAccessModule).Assembly,
                typeof(DefaultContext).Assembly,
                webApiAssembly
            });

            // ===== PIPELINE BEHAVIORS =====
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            return Task.CompletedTask;
        }
    }
}