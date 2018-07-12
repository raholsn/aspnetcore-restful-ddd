using System;
using System.Collections.Generic;
using System.Text;

namespace API.Contracts.Headers
{
    public class Pagination
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        

        //totalCount = pagination.TotalCount,
        //pageSize = pagination.PageSize,
        //currentPage = pagination.CurrentPage,
        //totalPages = pagination.TotalPages,
        //previousPageLink = pagination.HasPrevious? ResourceKey + "?pagenumber=" + (pageNumber - 1) + "&pageSize=" + pageSize : null,
        //nextPageLink = pagination.HasNext? ResourceKey + "?pagenumber=" + (pageNumber + 1) + "&pageSize=" + pageSize : null,
    }
}
