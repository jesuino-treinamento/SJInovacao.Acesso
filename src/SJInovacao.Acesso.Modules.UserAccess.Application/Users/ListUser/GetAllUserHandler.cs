using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.ListUser
{
    public class GetAllUserHandler : IRequestHandler<GetAllUsersQuery, PaginatedList<UserResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUserHandler(IUserRepository userRepository, IMapper mapper)
            => (_userRepository, _mapper) = (userRepository, mapper);

        public async Task<PaginatedList<UserResult>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllPaginatedAsync(
                request.Page,
                request.Size,
                request.Order);

            return new(
                _mapper.Map<List<UserResult>>(users.Items),
                users.TotalCount,
                users.PageNumber,
                users.PageSize);
        }
    }
}
