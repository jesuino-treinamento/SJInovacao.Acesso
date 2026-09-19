using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IUserPermissionRepository
    {
        // Criar vínculo entre usuário e permissão
        Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct);

        // Alterar status da permissão de um usuário
        Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct);

        // Remover vínculo (ou marcar como inativo, dependendo da regra de negócio)
        Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct);

        // Listar todas as permissões de um usuário
        Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct);

        // Listar todos os usuários que possuem uma permissão específica
        Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct);

        // Listar todos os usuários com suas permissões
        Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct);

        // Consultar um usuário específico com suas permissões
        Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct);

        Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId, CancellationToken ct);
    }

}
