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
}
