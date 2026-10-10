namespace SJInovacao.Acesso.UnitTests.Repositories
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Abstractions;
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
        private static DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DefaultContext(options);
        }

        private static PermissionRepository CreateRepository(DefaultContext context)
            => new(context, NullLogger<PermissionRepository>.Instance);

        private static Permission CreatePermission(string name, bool isActive = true)
            => new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };

        // =========================
        // CREATE
        // =========================
        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveSalvar()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var permission = CreatePermission("UnitTestPermissao");

            await repo.CreateAsync(permission, CancellationToken.None);

            var resultado = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            resultado.Should().NotBeNull();
            resultado!.Name.Should().Be("UnitTestPermissao");
        }

        [Fact]
        public async Task CreateAsync_ComNomeDuplicado_DeveLancarDomainException()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            await repo.CreateAsync(CreatePermission("Duplicado"), CancellationToken.None);

            Func<Task> act = () => repo.CreateAsync(CreatePermission("Duplicado"), CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("A permission with the same name already exists");
        }

        // =========================
        // UPDATE
        // =========================
        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizar()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var permission = CreatePermission("Original");
            await repo.CreateAsync(permission, CancellationToken.None);

            permission.Name = "Atualizada";
            await repo.UpdateAsync(permission, CancellationToken.None);

            var atualizada = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            atualizada!.Name.Should().Be("Atualizada");
        }

        [Fact]
        public async Task UpdateAsync_ComNomeDuplicado_DeveLancarDomainException()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var p1 = CreatePermission("Original");
            var p2 = CreatePermission("Existente");
            await repo.CreateAsync(p1, CancellationToken.None);
            await repo.CreateAsync(p2, CancellationToken.None);

            p1.Name = "Existente";

            Func<Task> act = () => repo.UpdateAsync(p1, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*Existente*");
        }

        // =========================
        // DELETE
        // =========================
        [Fact]
        public async Task DeleteAsync_ComIdExistente_DeveRetornarTrue()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var permission = CreatePermission("ParaDeletar");
            await repo.CreateAsync(permission, CancellationToken.None);

            var resultado = await repo.DeleteAsync(permission.Id, CancellationToken.None);

            resultado.Should().BeTrue();

            var apagada = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            apagada.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var resultado = await repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

            resultado.Should().BeFalse();
        }

        // =========================
        // DESATIVAR
        // =========================
        [Fact]
        public async Task DesativarAsync_ComIdValido_DeveMarcarComoInativo()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var permission = CreatePermission("ParaDesativar");
            await repo.CreateAsync(permission, CancellationToken.None);

            var resultado = await repo.DesativarAsync(permission.Id, CancellationToken.None);

            resultado.Should().BeTrue();

            var desativada = await repo.GetByIdAsync(permission.Id, CancellationToken.None);
            desativada!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DesativarAsync_ComIdInexistente_DeveRetornarFalse()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var resultado = await repo.DesativarAsync(Guid.NewGuid(), CancellationToken.None);

            resultado.Should().BeFalse();
        }

        // =========================
        // GET BY NAME
        // =========================
        [Fact]
        public async Task GetNameAsync_ComNomeExistente_DeveRetornarTrue()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            await repo.CreateAsync(CreatePermission("NomeExistente"), CancellationToken.None);

            var existe = await repo.GetNameAsync("NomeExistente", CancellationToken.None);

            existe.Should().BeTrue();
        }

        [Fact]
        public async Task GetNameAsync_ComNomeInexistente_DeveRetornarFalse()
        {
            using var context = CreateContext();
            var repo = CreateRepository(context);

            var existe = await repo.GetNameAsync("NomeQueNaoExiste", CancellationToken.None);

            existe.Should().BeFalse();
        }
    }
}