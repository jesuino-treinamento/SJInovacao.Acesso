using AutoMapper;
using FluentAssertions;
using Moq;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.UnitTests.Permissions
{
    public class CreatePermissionHandlerTests
    {
        private readonly Mock<IPermissionRepository> _repoMock;
        private readonly IMapper _mapper;
        private readonly CreatePermissionHandler _handler;

        public CreatePermissionHandlerTests()
        {
            _repoMock = new Mock<IPermissionRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(CreatePermissionHandler).Assembly);
            });
            _mapper = config.CreateMapper();

            _handler = new CreatePermissionHandler(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ComandoValido_DeveCriarPermissao()
        {
            // Arrange
            var command = new CreatePermissionCommand(
                Guid.NewGuid(),
                "Permissao_Teste",
                "Descrição de teste");

            _repoMock.Setup(r => r.GetNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Permission p, CancellationToken _) => p);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Permissao_Teste");
            result.IsActive.Should().BeTrue();

            _repoMock.Verify(
                r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_NomeDuplicado_DevePropagarDomainException()
        {
            // Arrange
            var command = new CreatePermissionCommand(
                Guid.NewGuid(),
                "Permissao_Duplicada",
                "Descrição");

            _repoMock
                .Setup(r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DomainException("A permission with the same name already exists"));

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*same name*");

            // Garante que só tentou criar uma vez
            _repoMock.Verify(
                r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}