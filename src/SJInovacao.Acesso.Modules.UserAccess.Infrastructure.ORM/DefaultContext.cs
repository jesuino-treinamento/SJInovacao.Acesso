using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SJInovacao.Acesso.Common.Auditing;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM
{
    /// <summary>
    /// DbContext do módulo Acesso.
    /// Contém entidades de Users, Permissions, Groups e Audit.
    /// </summary>
    public class DefaultContext : DbContext
    {
        private readonly IAuditService? _auditService;

        // =========================
        // DbSets
        // =========================
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Phone> Phones { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public DbSet<GroupPermission> GroupPermissions { get; set; } = null!;
        public DbSet<UserGroup> UserGroup { get; set; } = null!;
        public DbSet<GroupsPermissions> GroupsPermissions { get; set; } = null!;
        public DbSet<UsersGroupsPermissions> UsersGroupsPermissions { get; set; } = null!;

        // =========================
        // Construtores
        // =========================

        /// <summary>
        /// Construtor usado em RUNTIME. O DI injeta o IAuditService.
        /// </summary>
        public DefaultContext(
            DbContextOptions<DefaultContext> options,
            IAuditService auditService)
            : base(options)
        {
            _auditService = auditService;
        }

        /// <summary>
        /// Construtor usado em DESIGN-TIME (migrations).
        /// Não recebe IAuditService — auditoria fica desativada nesse contexto.
        /// </summary>
        public DefaultContext(DbContextOptions<DefaultContext> options)
            : base(options)
        {
            _auditService = null;
        }

        // =========================
        // Modelo
        // =========================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações do módulo Acesso (mesmo assembly)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configuração do AuditLog vem do Common (assembly diferente)
            modelBuilder.ApplyConfiguration(new AuditLogEntityConfiguration());
        }

        // =========================
        // SaveChanges — Auditoria automática
        // =========================

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_auditService is not null)
            {
                var auditLogs = _auditService.CaptureChanges(this);

                if (auditLogs.Count > 0)
                {
                    await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            if (_auditService is not null)
            {
                var auditLogs = _auditService.CaptureChanges(this);

                if (auditLogs.Count > 0)
                {
                    AuditLogs.AddRange(auditLogs);
                }
            }

            return base.SaveChanges();
        }
    }

    /// <summary>
    /// Factory usada pelo `dotnet ef` em design-time (migrations).
    /// </summary>
    public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
    {
        public DefaultContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            if (environment == "true")
            {
                configurationBuilder.AddJsonFile("appsettings.Docker.json", optional: false, reloadOnChange: true);
            }
            else
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }

            IConfigurationRoot configuration = configurationBuilder.Build();

            var builder = new DbContextOptionsBuilder<DefaultContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("SJInovacao.Acesso.Database"));

            // Chama o construtor de 1 parâmetro (design-time)
            return new DefaultContext(builder.Options);
        }
    }
}