namespace SJInovacao.Acesso.IntegrationTests.Commands
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
    using AutoMapper;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CreatePermissionCommandIntegrationTests
    {
        private DefaultContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new DefaultContext(options);
        }

        [Fact]
        public async Task Deve_Criar_Permissao_Com_Sucesso()
        {
            using var context = CreateContext();
            var repo = new PermissionRepository(context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatePermissionCommand, Permission>();
                cfg.CreateMap<Permission, SJInovacao.Acesso.Modules.UserAccess.Application.DTOs.PermissionDto>();
            });
            var mapper = mapperConfig.CreateMapper();

            var handler = new CreatePermissionHandler(repo, mapper);

            var command = new CreatePermissionCommand(Guid.NewGuid(), $"Permissao_{Guid.NewGuid()}", "Teste");
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Contain("Permissao");
            result.IsActive.Should().BeTrue();
        }
    }
}
