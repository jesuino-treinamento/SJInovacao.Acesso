namespace SJInovacao.Acesso.Modules.UserAccess.Application.Configuration.Paged
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult(List<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
    //public class PagedResult<T>
    //{
    //    public List<T> Items { get; set; } = new();
    //    public int TotalCount { get; set; }
    //    public int Page { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages { get; set; }
    //    public bool HasNextPage => Page < TotalPages;
    //    public bool HasPreviousPage => Page > 1;
    //}
}
