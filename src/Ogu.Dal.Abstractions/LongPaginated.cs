using System;
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
        public LongPaginated()
        {
            PagingInfo = new x64.PagingInfo();
            Items = Enumerable.Empty<TModel>();
        }

        public LongPaginated(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages, TModel[] items)
        {
            Items = items = items ?? Array.Empty<TModel>();

            PagingInfo = new x64.PagingInfo(pageIndex, items.Length, itemsPerPage, totalItems, rangeOfPages);
        }

        public LongPaginated(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages, IList<TModel> items)
        {
            Items = items = items ?? Array.Empty<TModel>();

            PagingInfo = new x64.PagingInfo(pageIndex, items.Count, itemsPerPage, totalItems, rangeOfPages);
        }

        public LongPaginated(long totalItems, TModel[] items) : this(1, totalItems, totalItems, 0, items)
        {
        }

        public LongPaginated(long totalItems, IList<TModel> items) : this(1, totalItems, totalItems, 0, items)
        {
        }

        public IPagingInfo<long> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; set; }
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