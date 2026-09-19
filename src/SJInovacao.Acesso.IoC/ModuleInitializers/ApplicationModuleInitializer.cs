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
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.IoC.ModuleInitializers
{
    public class ApplicationModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            // Registrando serviços necessários antecipadamente para permitir uso do DbContext
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, HybridPolicyProvider>();
            //builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            //builder.Services.AddSingleton<IAuthorizationPolicyProvider, GroupPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, GroupAuthorizationHandler>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IUserContext, UserContext>();

            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM")
                )
            );

            builder.Services.AddAutoMapper(typeof(ApplicationModuleInitializer).Assembly);

            // Registrar validators do FluentValidation a partir dos assemblies carregados
            // Substitui AddValidatorsFromAssemblies (pode faltar referência/namespace) por registro manual:
            // 1. Carregar assemblies relevantes.
            // 2. Enumerar tipos não abstratos.
            // 3. Encontrar interfaces que implementam IValidator<T>.
            // 4. Registrar cada par (IValidator<T>, Implementation) como transient no DI.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToArray();

            var validatorMappings = assemblies
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Service = i })
                .Where(x => x.Service.IsGenericType && x.Service.GetGenericTypeDefinition() == typeof(IValidator<>))
                .ToList();

            foreach (var mapping in validatorMappings)
            {
                builder.Services.AddTransient(mapping.Service, mapping.Implementation);
            }

            // Registrar o pipeline de validação (aplica validação antes dos handlers do MediatR)
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            // Não chamar Initialize de novo para evitar registros duplicados
            return Task.CompletedTask;
        }
    }
}

