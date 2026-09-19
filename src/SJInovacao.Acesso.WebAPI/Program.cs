//using SJInovacao.Acesso.IoC;
//using Serilog;
//using SJInovacao.Acesso.WebAPI.Middleware;
//using SJInovacao.Acesso.Common.Logging;
//using SJInovacao.Acesso.Common.HealthChecks;
//using SJInovacao.Acesso.Common.Middleware;
//using SJInovacao.Acesso.Common.Security;

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        try
//        {
//            Log.Information("Starting web application");

//            var builder = WebApplication.CreateBuilder(args);

//            builder.AddDefaultLogging();

//            builder.Services.AddEndpointsApiExplorer();
//            builder.AddBasicHealthChecks();

//            builder.Services.AddSwaggerGen();

//            builder.Services.AddAuthorization();

//            await builder.RegisterDependenciesAsync();

//            builder.Services.AddMediatR(cfg =>
//            {
//                cfg.RegisterServicesFromAssemblies(
//                    AppDomain.CurrentDomain.GetAssemblies()
//                        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
//                        .ToArray()
//                );
//            });

//            builder.Services.AddJwtAuthentication(builder.Configuration);
//            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
//                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
//                .ToArray()
//            );

//            var app = builder.Build();

//            app.UseMiddleware<ValidationExceptionMiddleware>();
//            app.UseMiddleware<UserContextMiddleware>();

//            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();
//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseBasicHealthChecks();

//            app.MapControllers();

//            app.Run();
//        }
//        catch (Exception ex)
//        {
//            Log.Fatal(ex, "Application terminated unexpectedly");
//        }
//        finally
//        {
//            Log.CloseAndFlush();
//        }
//    }
//}

using SJInovacao.Acesso.IoC;
using Serilog;
using SJInovacao.Acesso.WebAPI.Middleware;
using SJInovacao.Acesso.Common.Logging;
using SJInovacao.Acesso.Common.HealthChecks;
using SJInovacao.Acesso.Common.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            // =============================
            // 🔵 LOGGING + HEALTHCHECKS
            // =============================
            builder.AddDefaultLogging();
            builder.AddBasicHealthChecks();

            //sb.AppendLine("namespace SJInovacao.Acesso.Common.Security.Authentication");

            builder.Services.AddEndpointsApiExplorer();

            // =============================
            // 🔐 AUTHENTICAÇÃO (JWT)
            // =============================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });

            // =============================
            // 🔒 AUTORIZAÇÃO (POLICIES + PERMISSIONS)
            // =============================
            builder.Services.AddAuthorization();  // NÃO DUPLICAR

            // =============================
            // 📦 SWAGGER + JWT BUTTON
            // =============================
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SJInovacao.Acesso.WebAPI",
                    Version = "v1"
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Digite: Bearer {seu_token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

            // =============================
            // 📦 IoC MÓDULOS (Application, Infrastructure, WebAPI)
            // =============================
            await builder.RegisterDependenciesAsync();

            // =============================
            // 🔧 MEDIATR
            // =============================
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                        .ToArray());
            });

            // =============================
            // 🔧 AUTOMAPPER
            // =============================
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToArray());

            var app = builder.Build();

            // garantir pasta de logs antes do Serilog tentar escrever
            var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs");
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            // validação rápida de configuração para diagnóstico claro
            var config = builder.Configuration;
            if (string.IsNullOrWhiteSpace(config["Jwt:SecretKey"]))
                throw new InvalidOperationException("Configuração ausente: 'Jwt:SecretKey'. Defina em appsettings.json ou variável de ambiente (Jwt__SecretKey).");

            // forçar captura de erros de startup
            builder.WebHost.CaptureStartupErrors(true);

            app.UseAuthentication();  // ✔ ORDEM CORRETA
            app.UseAuthorization();   // ✔ ORDEM CORRETA

            // =============================
            // 🌐 MIDDLEWARES
            // =============================
            app.UseMiddleware<ValidationExceptionMiddleware>();
            app.UseMiddleware<UserContextMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // mostra stacktrace amigável em dev
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else if (app.Environment.EnvironmentName == "Docker")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();            

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
