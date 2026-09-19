using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM
{
    public class DefaultContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AccountPayable> AccountPayables { get; set; }
        public DbSet<AccountReceivable> AccountReceivables { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<CashMovement> CashMovements { get; set; }
        public DbSet<CashRegister> CashRegisters { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePayment> EmployeePayments { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<SubCatalog> SubCatalogs { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<TimeBank> TimeBanks { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<TimeSheetOvertime> TimeSheetOvertimes { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }

        public DbSet<UserGroup> UserGroup { get; set; }
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
