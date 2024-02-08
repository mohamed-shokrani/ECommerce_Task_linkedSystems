using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Helper;

public class Pagination<T> where T : class
{
    public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
    {
        PageIndex = pageIndex;
        Count = count;
        PageSize = pageSize;
        Data = data;
    }

    public int PageIndex { get; set; }

    public int Count { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> Data { get; set; }
    
}
