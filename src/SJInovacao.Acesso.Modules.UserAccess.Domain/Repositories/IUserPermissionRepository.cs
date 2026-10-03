using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    /// <summary>
    /// Interface para gerenciar relacionamentos entre usuários e permissões.
    /// </summary>
    public interface IUserPermissionRepository
    {
        /// <summary>
        /// Criar vínculo entre usuário e permissão
        /// </summary>
        Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct);

        /// <summary>
        /// Alterar status da permissão de um usuário
        /// </summary>
        Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct);

        /// <summary>
        /// Remover vínculo entre usuário e permissão
        /// </summary>
        Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct);

        /// <summary>
        /// Listar todas as permissões de um usuário específico
        /// </summary>
        Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Listar todos os usuários que possuem uma permissão específica
        /// </summary>
        Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct);

        /// <summary>
        /// Listar todos os usuários com suas permissões carregadas
        /// </summary>
        Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct);

        /// <summary>
        /// Consultar um usuário específico com suas permissões
        /// </summary>
        Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct);

        /// <summary>
        /// Verificar se um usuário possui uma permissão específica
        /// </summary>
        Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId, CancellationToken ct);
    }
}