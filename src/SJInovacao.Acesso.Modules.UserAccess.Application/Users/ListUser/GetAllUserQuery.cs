using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.ListUser
{
    public class GetAllUsersQuery : IRequest<PaginatedList<UserResult>>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "username, email";

        public ValidationResultDetail Validate()
        {
            return new ValidationResultDetail(new GetAllUsersQueryValidator().Validate(this));
        }
    }
}
