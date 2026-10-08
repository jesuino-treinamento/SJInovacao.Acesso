namespace SJInovacao.Acesso.UnitTests.Queries
{
    using FluentAssertions;
    using Moq;
    using AutoMapper;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions;
    using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ListPermissionsHandlerTests
    {
        [Fact]
        public async Task Deve_Mapear_E_Retornar_Permissoes()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new DefaultContext(options);
            context.Permissions.Add(new Permission { Id = Guid.NewGuid(), Name = "Permissao1", IsActive = true, CreatedAt = DateTime.UtcNow });
            context.Permissions.Add(new Permission { Id = Guid.NewGuid(), Name = "Permissao2", IsActive = true, CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Permission, PermissionDto>();
            });
            var mapper = mapperConfig.CreateMapper();

            var handler = new ListPermissionsHandler(context, mapper);
            var query = new ListPermissionsQuery();
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Should().BeOfType<PermissionDto>();
        }
    }
}
