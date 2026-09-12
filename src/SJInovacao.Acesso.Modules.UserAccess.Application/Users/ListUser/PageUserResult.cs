namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.ListUser
{
    public class PageUserResult
    {
        public IEnumerable<UserResult?> Data { get; set; } = new List<UserResult?>();
        public int TotalItems { get; set; } = 0;
        public int CurrentPage { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
    }
}
