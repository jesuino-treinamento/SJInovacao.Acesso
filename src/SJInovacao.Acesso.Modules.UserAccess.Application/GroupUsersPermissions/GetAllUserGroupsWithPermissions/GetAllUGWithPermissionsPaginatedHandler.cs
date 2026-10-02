using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUGWithPermissionsPaginatedHandler
    : IRequestHandler<GetAllUGWithPermissionsPaginatedQuery, PaginatedList<GroupUsersPermissionResult>>
    {
        private readonly IGroupPermissionUserRepository _repository;
        private readonly IMapper _mapper;

        public GetAllUGWithPermissionsPaginatedHandler(IGroupPermissionUserRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);

        public async Task<PaginatedList<GroupUsersPermissionResult>> Handle(
            GetAllUGWithPermissionsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var queryable = _repository.Query();

            if (request.OnlyActiveUsers)
                queryable = queryable.Where(g => !g.Group.UsersGroupsPermissions.Any(ugp => !ugp.IsActive));

            if (!string.IsNullOrWhiteSpace(request.PermissionNameFilter))
                queryable = queryable.Where(g => g.Group.UsersGroupsPermissions
                    .Any(p => EF.Functions.ILike(p.Permission.Name, $"%{request.PermissionNameFilter}%")));

            var users = await _repository.GetAllPaginatedAsync(request.Page, request.Size, request.Order, cancellationToken);

            var items = await queryable
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .Select(g => new GroupUsersPermissionResult
                {
                    UserId = g.UserId,
                    UserName = g.User.Name.ToString(),
                    GroupId = g.Group.Id,
                    GroupName = g.Group.Name,
                    Permissions = g.Group.UsersGroupsPermissions
                        .Select(p => new PermissionDto
                        {
                            Id = p.Permission.Id,
                            Name = p.Permission.Name,
                            Description = p.Permission.Description,
                            IsActive = p.IsActive,
                            CreatedAt = p.CreatedAt,
                            UpdatedAt = p.UpdatedAt
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            //var _itens = _mapper.Map<List<UserGroupDTO>>(users.Items.Select(x => x.Group));

            return new PaginatedList<GroupUsersPermissionResult>(
                items,
                users.TotalCount,
                users.PageNumber,
                users.PageSize);
        }
    }

}
