namespace SJInovacao.Acesso.IntegrationTests.Repositories
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
    using Xunit;

    public class PermissionRepositoryTests
    {
        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // sempre único
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
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
                Name = $"Relatorios_{Guid.NewGuid()}",
                Description = "Acesso a relatórios",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.CreateAsync(permission, CancellationToken.None);
            var result = await repo.GetByIdAsync(permission.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Name.Should().Contain("Relatorios");
        }

        [Fact]
        public async Task Deve_Atualizar_Permissao()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = $"Antiga_{Guid.NewGuid()}",
                Description = "Desc antiga",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            await repo.CreateAsync(permission, CancellationToken.None);

            permission.Name = $"Nova_{Guid.NewGuid()}";
            permission.IsActive = true;
            await repo.UpdateAsync(permission, CancellationToken.None);

            var updated = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            updated!.Name.Should().Contain("Nova");
            updated.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Deve_Excluir_Permissao()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = $"Excluir_{Guid.NewGuid()}",
                Description = "Teste",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await repo.CreateAsync(permission, CancellationToken.None);

            await repo.DeleteAsync(permission.Id, CancellationToken.None);
            var result = await repo.GetByIdAsync(permission.Id, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Deve_Listar_Todas_Permissoes()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            await repo.CreateAsync(new Permission { Id = Guid.NewGuid(), Name = $"P1_{Guid.NewGuid()}", IsActive = true, CreatedAt = DateTime.UtcNow }, CancellationToken.None);
            await repo.CreateAsync(new Permission { Id = Guid.NewGuid(), Name = $"P2_{Guid.NewGuid()}", IsActive = true, CreatedAt = DateTime.UtcNow }, CancellationToken.None);

            var all = await repo.GetAllAsync(CancellationToken.None);

            all.Should().HaveCount(2);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Nome_Duplicado()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var name = $"Duplicado_{Guid.NewGuid()}";
            var p1 = new Permission { Id = Guid.NewGuid(), Name = name, IsActive = true, CreatedAt = DateTime.UtcNow };
            var p2 = new Permission { Id = Guid.NewGuid(), Name = name, IsActive = true, CreatedAt = DateTime.UtcNow };

            await repo.CreateAsync(p1, CancellationToken.None);

            Func<Task> act = async () => await repo.CreateAsync(p2, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
