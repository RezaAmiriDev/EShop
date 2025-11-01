using System.Text;


namespace Common.Pagination
{
    public class PagedResponse<TData>
    {
        public PagedResponse(int pageNumber, int totalRecords, TData data)
        {
            var _StartIndex = 0;
            if (pageNumber == 0 || pageNumber == 1)
            {
                _StartIndex = 0;
                pageNumber = 1;
            }
            else
            {
                var Skip = pageNumber - 1;
                _StartIndex = 10 * Skip;
            }
            if (pageNumber == 1)
            {
                PreviousPage = pageNumber;
            }
            else
            {
                PreviousPage = pageNumber - 1;
            }

            PageNumber = pageNumber;
            PageSize = 10;
            FirstPage = 1;
            LastPage = totalRecords / PageSize;
            TotalPages = countPages(totalRecords, PageSize);
            TotalRecords = totalRecords;
            Data = data;
            NumberReturned = 10;
            StartIndex = _StartIndex;
            NextPage = pageNumber++;
            if (totalRecords <= 10)
            {
                LastPage = 1;
                NextPage = 1;
                TotalPages = 1;
            }
        }
        public int PageNumber { get; set; }
        public int PageSize { get; } = 10;
        public int FirstPage { get; }
        public int LastPage { get; }
        public int TotalPages { get; }
        public int TotalRecords { get; }
        public int NextPage { get; }
        public int StartIndex { get; }
        public int NumberReturned { get; }
        public int PreviousPage { get; }
        public TData Data { get; set; }

        public int countPages(int totalRecords, int recordsPerPage)
        {
            return ((totalRecords - 1) / recordsPerPage) + 1;
        }
    }

    public class PagedRequest<TData>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int StartIndex { get; set; } = 0;
        public TData? Data { get; set; }
    }

    public class PagedResult<TData>
    {
        public PagedResult() { }

        // ctor که از PagedResponse<TData> مقداردهی می‌کند
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
