using System.Text;


namespace Common.Pagination
{
    public class PagedResponse<TData>
    {
        public PagedResponse(int pageNumber, int pageSize, int totalRecords, TData data)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 12;

            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Data = data;
            TotalPages = CalculateTotalPages(totalRecords, pageSize);
            StartIndex = (pageNumber - 1) * PageSize;
            FirstPage = 1;
            LastPage = TotalPages;
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1;
            NextPage = pageNumber < TotalPages ?  pageNumber + 1 : TotalPages;
            NumberReturned = Math.Min(pageSize, totalRecords - StartIndex);
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int FirstPage { get; set; }
        public int LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int NextPage { get; set; }
        public int StartIndex { get; set; }
        public int NumberReturned { get; }
        public int PreviousPage { get; set; }
        public TData Data { get; set; }

        public int CalculateTotalPages(int totalRecords, int pageSize)
        {
            if (pageSize <= 0) return 0;
            return (int)Math.Ceiling((double)totalRecords / pageSize);
        }
    }

    public class PagedRequest<TData>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int StartIndex { get; set; } = 0;
        public TData? Data { get; set; }
    }

    public class PagedResult<TData>
    {
        public PagedResult() { }

        public PagedResult(PagedResponse<TData> src)
        {
            if (src == null) return;
            PageNumber = src.PageNumber;
            PageSize = src.PageSize;
            FirstPage = src.FirstPage;
            LastPage = src.LastPage;
            TotalPages = src.TotalPages;
            TotalRecords = src.TotalRecords;
            NextPage = src.NextPage;
            StartIndex = src.StartIndex;
            NumberReturned = src.NumberReturned;
            PreviousPage = src.PreviousPage;
            Data = src.Data;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int FirstPage { get; set; }
        public int LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int NextPage { get; set; }
        public int StartIndex { get; set; }
        public int NumberReturned { get; set; }
        public int PreviousPage { get; set; }
        public TData? Data { get; set; }
    }
}
