using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM
{
    /// <summary>
    /// DbContext otimizado para o módulo de Acesso à Usuários e Permissões.
    /// Contém apenas entidades essenciais para reduzir complexidade.
    /// </summary>
    /// <summary>
    /// DbContext otimizado para o módulo de Acesso à Usuários e Permissões.
    /// Contém apenas entidades essenciais para reduzir complexidade.
    /// </summary>
    public class DefaultContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<GroupsPermissions> GroupsPermissions { get; set; }
        public DbSet<UsersGroupsPermissions> UsersGroupsPermissions { get; set; }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
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

            return new DefaultContext(builder.Options);
        }
    }
}
