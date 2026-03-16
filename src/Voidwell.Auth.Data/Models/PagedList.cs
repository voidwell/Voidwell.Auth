using System.Collections.Generic;

namespace Voidwell.Auth.Data.Models;

public class PagedList<T> where T : class
{
    public PagedList(IEnumerable<T> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Data { get; private set; }
    public int TotalCount { get; private set; }
}
