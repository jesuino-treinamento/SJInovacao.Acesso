using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using Serilog.Context;
using SJInovacao.Acesso.Common.HealthChecks;
using SJInovacao.Acesso.Common.Logging;
using SJInovacao.Acesso.Common.Middleware;
using SJInovacao.Acesso.IoC;
using SJInovacao.Acesso.WebAPI.Middleware;
using System.Text;

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        try
//        {
//            Log.Information("Starting web application");

//            var builder = WebApplication.CreateBuilder(args);

//            // ===== LOGGING COM CORRELATION ID =====
//            //Log.Logger = new LoggerConfiguration()
//            //    .MinimumLevel.Information()
//            //    .WriteTo.Console(
//            //        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
//            //    .WriteTo.File(
//            //        path: Path.Combine(AppContext.BaseDirectory, "logs", "app-.txt"),
//            //        rollingInterval: RollingInterval.Day,
//            //        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}",
//            //        retainedFileCountLimit: 30)
//            //    .Enrich.FromLogContext()
//            //    .Enrich.WithProperty("Application", "SJInovacao.Acesso.WebAPI")
//            //    .CreateLogger();

//            //builder.Host.UseSerilog();

//            // ===== VALIDATION CONFIG =====
//            var jwtSecret = builder.Configuration["Jwt:SecretKey"];
//            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//            if (string.IsNullOrWhiteSpace(jwtSecret))
//                throw new InvalidOperationException("❌ JWT:SecretKey não configurado em appsettings.json");

//            if (string.IsNullOrWhiteSpace(connectionString))
//                throw new InvalidOperationException("❌ DefaultConnection não configurada em appsettings.json");

//            Log.Information("✅ Configurações validadas com sucesso");



//            // ===== SERVICES =====
//            builder.Services.AddEndpointsApiExplorer();
//            //builder.Services.AddCors(options =>
//            //{
//            //    options.AddPolicy("AllowAll", policy =>
//            //    {
//            //        policy.AllowAnyOrigin()
//            //              .AllowAnyMethod()
//            //              .AllowAnyHeader();
//            //    });
//            //});

//            // ===== HEALTH CHECKS =====
//            builder.AddDefaultLogging();
//            builder.AddBasicHealthChecks();            

//            // ===== SWAGGER/OPENAPI =====
//            builder.Services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo 
//                { 
//                    Title = "SJInovacao.Acesso API", 
//                    Version = "v1",
//                    Description = "API de Gerenciamento de Acesso e Permissões",
//                    Contact = new OpenApiContact { Name = "SJInovacao" }
//                });

//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Type = SecuritySchemeType.Http,
//                    Scheme = "bearer",
//                    BearerFormat = "JWT",
//                    Description = "JWT Authorization header using the Bearer scheme"
//                });

//                c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        new string[] { }
//                    }
//                });
//            });

//            // ===== AUTHENTICATION =====
//            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(options =>
//                {
//                    options.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
//                        ValidateIssuer = false,
//                        ValidateAudience = false,
//                        ValidateLifetime = true,
//                        ClockSkew = TimeSpan.Zero
//                    };

//                    options.Events = new JwtBearerEvents
//                    {
//                        OnAuthenticationFailed = context =>
//                        {
//                            Log.Warning("🔐 Falha na autenticação JWT: {Message}", context.Exception.Message);
//                            return Task.CompletedTask;
//                        },
//                        OnTokenValidated = context =>
//                        {
//                            var claim = context.Principal?.FindFirst("sub")?.Value;
//                            LogContext.PushProperty("UserId", claim);
//                            return Task.CompletedTask;
//                        }
//                    };
//                });

//            //builder.MediatRModule();

//            builder.Services.AddAuthorization();

//            // ===== AUTOMAPPER =====
//            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
//                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
//                .ToArray()
//            );

//            // ===== POLLY RESILIENCE POLICY =====
//            var resiliencePolicy = Policy
//                .Handle<Exception>()
//                .WaitAndRetryAsync(
//                    retryCount: 3,
//                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100),
//                    onRetry: (outcome, timespan, retryCount, context) =>
//                    {
//                        Log.Warning("🔄 Tentativa {RetryCount}/3 após {Delay}ms", retryCount, timespan.TotalMilliseconds);
//                    });

//            builder.Services.AddSingleton<IAsyncPolicy>(resiliencePolicy);

//            // ===== DI REGISTRATION (MediatR SERÁ REGISTRADO AQUI) =====
//            await builder.RegisterDependenciesAsync();

//            // ===== BUILD APP =====
//            var app = builder.Build();

//            // ===== CREATE LOGS FOLDER =====
//            var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
//            if (!Directory.Exists(logsPath))
//                Directory.CreateDirectory(logsPath);

//            // ===== MIDDLEWARE =====
//            app.UseMiddleware<CorrelationIdMiddleware>();
//            app.UseMiddleware<ValidationExceptionMiddleware>();
//            app.UseMiddleware<UserContextMiddleware>();

//            if (app.Environment.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage(); // mostra stacktrace amigável em dev
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }
//            else if (app.Environment.EnvironmentName == "Docker")
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();
//            //app.UseCors("AllowAll");
//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseBasicHealthChecks();
//            app.UseAdvancedHealthChecks();

//            app.MapControllers();

//            Log.Information("🚀 WebAPI iniciada com sucesso");
//            app.Run();
//        }
//        catch (Exception ex)
//        {
//            Log.Fatal(ex, "💥 Aplicação terminada inesperadamente");
//            throw;
//        }
//        finally
//        {
//            Log.Information("🛑 Encerrando aplicação");
//            await Log.CloseAndFlushAsync();
//        }
//    }
//}

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

                // 🔑 API Key
                var apiKeySecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "Digite sua API Key",
                    Name = "X-API-KEY", // nome do header
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                };

                c.AddSecurityDefinition("ApiKey", apiKeySecurityScheme);

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { apiKeySecurityScheme, Array.Empty<string>() },
                    { securityScheme, Array.Empty<string>() }
                });
            });

            // =============================
            // 📦 IoC MÓDULOS (Application, Infrastructure, WebAPI)
            // =============================
            await builder.RegisterDependenciesAsync();            

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


            // 🔐 PROTEÇÃO DO SWAGGER EM PRODUÇÃO
            if (app.Environment.IsProduction())                       
            {
                app.UseMiddleware<SwaggerBasicAuthMiddleware>();
            }
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage(); // mostra stacktrace amigável em dev
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            //else if (app.Environment.EnvironmentName == "Docker")
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker" || app.Environment.IsProduction())
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
