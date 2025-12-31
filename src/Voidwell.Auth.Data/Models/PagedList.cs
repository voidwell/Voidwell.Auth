using System.Collections.Generic;

namespace Voidwell.Auth.Data.Models;

public class PagedList<T> where T : class
{
    public PagedList(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Data { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
}
