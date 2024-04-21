using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.Abstractions
{
    public interface IPaginated<out TModel>
    {
        IPagingInfo<int> PagingInfo { get; }
        IEnumerable<TModel> Items { get; }
    }

    public class Paginated<TModel> : IPaginated<TModel>
    {
        private Paginated()
        {
            PagingInfo = new x86.PagingInfo();
            Items = Enumerable.Empty<TModel>();
        }

        internal Paginated(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages, IEnumerable<TModel> items)
        {
            var list = items as IList<TModel> ?? items.ToArray();

            if (list.Count > totalItems)
            {
                totalItems = list.Count;
            }

            if (list.Count > itemsPerPage)
            {
                itemsPerPage = list.Count;
            }

            Items = list;
  
            PagingInfo = new x86.PagingInfo(pageIndex, list.Count, itemsPerPage, totalItems, rangeOfPages);
        }

        internal Paginated(int totalItems, IEnumerable<TModel> items)
        {
            var list = items as IList<TModel> ?? items.ToArray();

            if (list.Count > totalItems)
            {
                totalItems = list.Count;
            }

            Items = list;

            PagingInfo = new x86.PagingInfo(1, list.Count, totalItems, totalItems, 0);
        }

        internal Paginated(IEnumerable<TModel> items)
        {
            var list = items as IList<TModel> ?? items.ToArray();

            Items = list;

            PagingInfo = new x86.PagingInfo(1, list.Count, list.Count, list.Count, 0);
        }

        public IPagingInfo<int> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; }

        public static IPaginated<TModel> Empty => new Paginated<TModel>();
    }

    public class PaginatedDto<TDto> : IPaginated<TDto>
    {
        internal PaginatedDto(IPagingInfo<int> pagingInfo, IEnumerable<TDto> items)
        {
            PagingInfo = pagingInfo;
            Items = items;
        }

        public IPagingInfo<int> PagingInfo { get; set; }
        public IEnumerable<TDto> Items { get; set; }
    }
}