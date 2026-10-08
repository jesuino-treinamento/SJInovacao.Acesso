namespace SJInovacao.Acesso.IntegrationTests.Validators
{
    using FluentAssertions;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
    using System;
    using Xunit;

    public class UpdatePermissionValidatorIntegrationTests
    {
        [Fact]
        public void Deve_Validar_Comando_Correto()
        {
            var validator = new UpdatePermissionValidator();
            var command = new UpdatePermissionCommand(Guid.NewGuid(), "PermissaoValida", "Descrição", true);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Deve_Falhar_Quando_Nome_Vazio()
        {
            var validator = new UpdatePermissionValidator();
            var command = new UpdatePermissionCommand(Guid.NewGuid(), "", "Descrição", true);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }
}
