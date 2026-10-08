namespace SJInovacao.Acesso.InfraTests.Repositories
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class PermissionRepositoryInfraTests
    {
        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DefaultContext(options);
        }

        [Fact]
        public async Task Deve_Salvar_E_Buscar_Permissao()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "InfraPermissao",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.CreateAsync(permission, CancellationToken.None);
            var result = await repo.GetByIdAsync(permission.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Name.Should().Be("InfraPermissao");
        }
    }
}
