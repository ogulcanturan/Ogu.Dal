using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.Abstractions
{
    public interface ILongPaginated<out TModel>
    {
        IPagingInfo<long> PagingInfo { get; }
        IEnumerable<TModel> Items { get; }
    }

    public class LongPaginated<TModel> : ILongPaginated<TModel>
    {
        private LongPaginated()
        {
            PagingInfo = new x64.PagingInfo();
            Items = Enumerable.Empty<TModel>();
        }

        internal LongPaginated(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages, IEnumerable<TModel> items)
        {
            var list = items as TModel[] ?? items.ToArray();

            if (list.LongLength > totalItems)
            {
                totalItems = list.LongLength;
            }

            if (list.LongLength > itemsPerPage)
            {
                itemsPerPage = list.LongLength;
            }

            Items = list;

            PagingInfo = new x64.PagingInfo(pageIndex, list.LongLength, itemsPerPage, totalItems, rangeOfPages);
        }

        internal LongPaginated(long totalItems, IEnumerable<TModel> items)
        {
            var list = items as TModel[] ?? items.ToArray();

            if (list.LongLength > totalItems)
            {
                totalItems = list.LongLength;
            }

            Items = list;

            PagingInfo = new x64.PagingInfo(1, list.LongLength, totalItems, totalItems, 0);
        }

        internal LongPaginated(IEnumerable<TModel> items)
        {
            var list = items as TModel[] ?? items.ToArray();

            Items = list;

            PagingInfo = new x64.PagingInfo(1, list.LongLength, list.LongLength, list.LongLength, 0);
        }

        public IPagingInfo<long> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; set; }

        public static ILongPaginated<TModel> Empty => new LongPaginated<TModel>();
    }

    public class LongPaginatedDto<TDto> : ILongPaginated<TDto>
    {
        public LongPaginatedDto() { }
        public LongPaginatedDto(IPagingInfo<long> pagingInfo, IEnumerable<TDto> items)
        {
            PagingInfo = pagingInfo;
            Items = items;
        }

        public IPagingInfo<long> PagingInfo { get; set; }
        public IEnumerable<TDto> Items { get; set; }
    }
}