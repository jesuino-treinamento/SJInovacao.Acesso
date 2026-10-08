namespace SJInovacao.Acesso.IntegrationTests.Queries
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using AutoMapper;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ListPermissionsHandlerIntegrationTests
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
        public async Task Deve_Listar_Todas_Permissoes()
        {
            using var context = CreateContext();

            context.Permissions.Add(new Permission { Id = Guid.NewGuid(), Name = $"P1_{Guid.NewGuid()}", IsActive = true, CreatedAt = DateTime.UtcNow });
            context.Permissions.Add(new Permission { Id = Guid.NewGuid(), Name = $"P2_{Guid.NewGuid()}", IsActive = true, CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Permission, SJInovacao.Acesso.Modules.UserAccess.Application.DTOs.PermissionDto>();
            });
            var mapper = mapperConfig.CreateMapper();

            var handler = new ListPermissionsHandler(context, mapper);
            var query = new ListPermissionsQuery();
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().HaveCount(2);
        }
    }
}
