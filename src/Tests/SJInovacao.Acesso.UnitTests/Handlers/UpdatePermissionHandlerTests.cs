using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.UnitTests.Handlers
{
    public class UpdatePermissionHandlerTests
    {
        private DefaultContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new DefaultContext(options);
        }

        [Fact]
        public async Task Deve_Atualizar_Permissao_Com_Sucesso()
        {
            var context = CreateContext("UnitTestDb_Sucesso");

            var existingPermission = new Permission { Id = Guid.NewGuid(), Name = "Antiga", IsActive = false };
            context.Permissions.Add(existingPermission);
            await context.SaveChangesAsync();

            var repoMock = new Mock<IPermissionRepository>();
            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission> { existingPermission });

            var loggerMock = new Mock<ILogger<UpdatePermissionHandler>>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<PermissionDto>(It.IsAny<Permission>()))
                      .Returns((Permission p) => new PermissionDto { Id = p.Id, Name = p.Name, IsActive = p.IsActive });

            var handler = new UpdatePermissionHandler(repoMock.Object, context, loggerMock.Object, mapperMock.Object);

            var command = new UpdatePermissionCommand(existingPermission.Id, "Nova", "Teste", true);
            var result = await handler.Handle(command, CancellationToken.None);

            result.Name.Should().Be("Nova");
            result.IsActive.Should().BeTrue();

            var updated = await context.Permissions.FindAsync(existingPermission.Id);
            updated!.Name.Should().Be("Nova");
            updated.IsActive.Should().BeTrue();
        }


        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Permissao_Nao_Encontrada()
        {
            var context = CreateContext("UnitTestDb_NotFound");

            var repoMock = new Mock<IPermissionRepository>();
            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission>()); // lista vazia

            var handler = new UpdatePermissionHandler(repoMock.Object, context, Mock.Of<ILogger<UpdatePermissionHandler>>(), Mock.Of<IMapper>());

            var command = new UpdatePermissionCommand(Guid.NewGuid(), "Nova", "Teste", true);

            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Nome_Duplicado()
        {
            var context = CreateContext("UnitTestDb_Duplicado");

            var repoMock = new Mock<IPermissionRepository>();
            var existingPermission = new Permission { Id = Guid.NewGuid(), Name = "Duplicado", IsActive = true };
            var anotherPermission = new Permission { Id = Guid.NewGuid(), Name = "Duplicado", IsActive = true };

            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission> { existingPermission, anotherPermission });

            var handler = new UpdatePermissionHandler(repoMock.Object, context, Mock.Of<ILogger<UpdatePermissionHandler>>(), Mock.Of<IMapper>());

            var command = new UpdatePermissionCommand(existingPermission.Id, "Duplicado", "Teste", true);

            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();
        }

        [Fact]
        public async Task Deve_Lancar_ValidationException_Quando_Comando_Invalido()
        {
            var context = CreateContext("UnitTestDb_Validation");

            var repoMock = new Mock<IPermissionRepository>();
            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission>());

            var handler = new UpdatePermissionHandler(repoMock.Object, context, Mock.Of<ILogger<UpdatePermissionHandler>>(), Mock.Of<IMapper>());

            var command = new UpdatePermissionCommand(Guid.NewGuid(), "", "Teste", true); // nome vazio

            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }


        [Fact]
        public async Task Deve_Chamar_Logger_E_Mapper()
        {
            var context = CreateContext("UnitTestDb_LoggerMapper");

            var existingPermission = new Permission { Id = Guid.NewGuid(), Name = "Antiga", IsActive = false };
            context.Permissions.Add(existingPermission);
            await context.SaveChangesAsync();

            var repoMock = new Mock<IPermissionRepository>();
            var loggerMock = new Mock<ILogger<UpdatePermissionHandler>>();
            var mapperMock = new Mock<IMapper>();

            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Permission> { existingPermission });

            mapperMock.Setup(m => m.Map<PermissionDto>(It.IsAny<Permission>()))
                      .Returns(new PermissionDto { Id = existingPermission.Id, Name = "Nova", IsActive = true });

            var handler = new UpdatePermissionHandler(repoMock.Object, context, loggerMock.Object, mapperMock.Object);

            var command = new UpdatePermissionCommand(existingPermission.Id, "Nova", "Teste", true);
            await handler.Handle(command, CancellationToken.None);

            loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(lvl => lvl == LogLevel.Information),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            mapperMock.Verify(m => m.Map<PermissionDto>(It.IsAny<Permission>()), Times.Once);
        }
    }
}
