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

            builder.Services.AddAuthorization();

            // ===== DATABASE =====
            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SJInovacao.Acesso.Database")
                )
            );


            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToArray());



            // ===== MEDIATR HANDLERS =====
            RegisterMediatRHandlers(builder.Services);

            


            // ===== VALIDATORS =====
            RegisterValidators(builder.Services);

            // ===== MEDIATR PIPELINE BEHAVIORS =====
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public Task InitializeAsync(WebApplicationBuilder builder)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registra handlers MediatR do assembly de Application
        /// </summary>
        private static void RegisterMediatRHandlers(IServiceCollection services)
        {
            try
            {
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
               .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
               .ToArray());

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                            .ToArray());
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao registrar MediatR handlers: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra validators FluentValidation com filtro rigoroso
        /// </summary>
        private static void RegisterValidators(IServiceCollection services)
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location) && 
                               (a.GetName().Name?.Contains("SJInovacao") ?? false))
                    .ToArray();

                var validatorMappings = new List<ValidatorMapping>();

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();
                        
                        foreach (var type in types)
                        {
                            // ✅ FILTRO RIGOROSO - APENAS tipos que herdam de AbstractValidator<T>
                            if (IsConcreteValidator(type))
                            {
                                var interfaces = type.GetInterfaces()
                                    .Where(i => i.IsGenericType && 
                                               i.GetGenericTypeDefinition() == typeof(IValidator<>))
                                    .ToList();

                                foreach (var iface in interfaces)
                                {
                                    validatorMappings.Add(new ValidatorMapping(iface, type));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Erro ao escanear assembly {assembly.GetName().Name}: {ex.Message}");
                    }
                }

                // Remover duplicatas
                var distinct = validatorMappings.DistinctBy(x => x.InterfaceType.FullName).ToList();

                // Registrar
                foreach (var mapping in distinct)
                {
                    services.AddTransient(mapping.InterfaceType, mapping.ImplementationType);
                }

                System.Diagnostics.Debug.WriteLine($"✅ {distinct.Count} validators registrados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Erro geral ao registrar validators: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se um tipo é um validator concreto (herda de AbstractValidator{T})
        /// Exclui tipos internos do FluentValidation
        /// </summary>
        private static bool IsConcreteValidator(Type type)
        {
            try
            {
                // ❌ Excluir tipos internos do FluentValidation
                if (type.Namespace?.StartsWith("FluentValidation") ?? false)
                    return false;

                // ❌ Excluir tipos abstratos e interfaces
                if (type.IsAbstract || type.IsInterface)
                    return false;

                // ❌ Excluir tipos genéricos abertos (Type<>)
                if (type.IsGenericTypeDefinition)
                    return false;

                // ✅ Verificar se herda de AbstractValidator<T>
                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType &&
                        baseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
                    {
                        return true;
                    }
                    baseType = baseType.BaseType;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private class ValidatorMapping
        {
            public Type InterfaceType { get; }
            public Type ImplementationType { get; }

            public ValidatorMapping(Type interfaceType, Type implementationType)
            {
                InterfaceType = interfaceType;
                ImplementationType = implementationType;
            }
        }
    }
}

