namespace RemsNG.Common.Models
{
    public class PageModel
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int totalPageCount { get; set; }
        public object data { get; set; }
    }

    public class PageModel<T>
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int totalPageCount { get; set; }
        public T data { get; set; }
    }
}
