namespace SJInovacao.Acesso.ApplicationTests.Commands
{
    using FluentAssertions;
    using Moq;
    using AutoMapper;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
    using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
    using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CreatePermissionHandlerTests
    {
        [Fact]
        public async Task Deve_Criar_Permissao_Com_Sucesso()
        {
            var repoMock = new Mock<IPermissionRepository>();
            var mapperMock = new Mock<IMapper>();

            var permission = new Permission { Id = Guid.NewGuid(), Name = "AppPermissao", IsActive = true };
            var dto = new PermissionDto { Id = permission.Id, Name = permission.Name, IsActive = permission.IsActive };

            repoMock.Setup(r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(permission);

            mapperMock.Setup(m => m.Map<Permission>(It.IsAny<CreatePermissionCommand>()))
                      .Returns(permission);

            mapperMock.Setup(m => m.Map<PermissionDto>(It.IsAny<Permission>()))
                      .Returns(dto);

            var handler = new CreatePermissionHandler(repoMock.Object, mapperMock.Object);

            var command = new CreatePermissionCommand(Guid.NewGuid(), "AppPermissao", "Descrição");
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Name.Should().Be("AppPermissao");
        }
    }
}
