using Microsoft.EntityFrameworkCore;

namespace Api.Helper
{
    public  class ApplyPagination<T> : List<T>
    {
        public static async Task<List<T>> CreateAsync(IQueryable<T> sourceData, int pageNumber, int pageSize)
        {

            var count = await sourceData.CountAsync();
            var items = await sourceData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return items;

        }
    }
}
