namespace SJInovacao.Acesso.UnitTests.Validators
{
    using FluentAssertions;
    using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
    using System;
    using Xunit;

    public class UpdatePermissionValidatorTests
    {
        [Fact]
        public void Deve_Aceitar_Comando_Valido()
        {
            var validator = new UpdatePermissionValidator();
            var command = new UpdatePermissionCommand(Guid.NewGuid(), "PermissaoValida", "Descrição", true);

            var result = validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Deve_Rejeitar_Nome_Vazio()
        {
            var validator = new UpdatePermissionValidator();
            var command = new UpdatePermissionCommand(Guid.NewGuid(), "", "Descrição", true);

            var result = validator.Validate(command);

            result.IsValid.Should().BeFalse();
        }
    }
}
