namespace SJInovacao.Acesso.UnitTests.Repositories
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class PermissionRepositoryUnitTests
    {
        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // banco isolado
                .Options;

            return new DefaultContext(options);
        }

        [Fact]
        public async Task Deve_Salvar_Permissao()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "UnitTestPermissao",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.CreateAsync(permission, CancellationToken.None);

            var result = await repo.GetByIdAsync(permission.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Name.Should().Be("UnitTestPermissao");
        }

        [Fact]
        public async Task Deve_Atualizar_Permissao()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Antiga",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            await repo.CreateAsync(permission, CancellationToken.None);

            permission.Name = "Nova";
            permission.IsActive = true;
            await repo.UpdateAsync(permission, CancellationToken.None);

            var updated = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            updated!.Name.Should().Be("Nova");
            updated.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Nome_Duplicado()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var name = "Duplicado";
            var p1 = new Permission { Id = Guid.NewGuid(), Name = name, IsActive = true, CreatedAt = DateTime.UtcNow };
            var p2 = new Permission { Id = Guid.NewGuid(), Name = name, IsActive = true, CreatedAt = DateTime.UtcNow };

            await repo.CreateAsync(p1, CancellationToken.None);

            Func<Task> act = async () => await repo.CreateAsync(p2, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("A permission with the same name already exists");
        }
        [Fact]
        public async Task Deve_Retornar_Nulo_Quando_Excluir_Permissao_Inexistente()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var idInexistente = Guid.NewGuid();

            await repo.DeleteAsync(idInexistente, CancellationToken.None);
            var result = await repo.GetByIdAsync(idInexistente, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
