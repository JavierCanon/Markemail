using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Eron.MailClient.SharedKernel.Modules.ValueObjects;

namespace Eron.MailClient.SharedKernel.Modules.Helpers.ListExtensions
{
    public static class QueryableExtensions
    {
        public static IPagedListResult<T> ToPagedList<T>(this IQueryable<T> source, int totalCount, int pageSize)
        {
            return new PagedListResult<T>().Create(source.ToList());
        }

        public static async Task<IPagedListResult<T>> ToPagedListAsync<T>(this IQueryable<T> source, int totalCount, int pageSize)
        {
            return new PagedListResult<T>().Create(await source.ToListAsync(), totalCount, pageSize);
        }
    }
}
