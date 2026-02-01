namespace HMS.Common.DTOs
{
    public class PaginatedList<T>
    {
        public PaginatedList(List<T> items, int pageNumer, int totalCount, int pageSize)
        {
            Items = items;
            PageNumber = pageNumer;
            TotalCount = totalCount;
            PageSize = pageSize;
        }

        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
