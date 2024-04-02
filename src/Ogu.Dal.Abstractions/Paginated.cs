using System;
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
        public Paginated()
        {
            PagingInfo = new x86.PagingInfo();
            Items = Enumerable.Empty<TModel>();
        }

        public Paginated(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages, TModel[] items)
        {
            Items = items = items ?? Array.Empty<TModel>();

            PagingInfo = new x86.PagingInfo(pageIndex, items.Length, itemsPerPage, totalItems, rangeOfPages);
        }

        public Paginated(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages, IList<TModel> items)
        {
            Items = items = items ?? new List<TModel>();

            PagingInfo = new x86.PagingInfo(pageIndex, items.Count, itemsPerPage, totalItems, rangeOfPages);
        }

        public Paginated(int totalItems, TModel[] items) : this(1, totalItems, totalItems, 0, items)
        {
        }

        public Paginated(int totalItems, IList<TModel> items) : this(1, totalItems, totalItems, 0, items)
        {
        }

        public IPagingInfo<int> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; }
    }

    public class PaginatedDto<TDto> : IPaginated<TDto>
    {
        public PaginatedDto(IPagingInfo<int> pagingInfo, IEnumerable<TDto> items)
        {
            PagingInfo = pagingInfo;
            Items = items;
        }

        public IPagingInfo<int> PagingInfo { get; set; }
        public IEnumerable<TDto> Items { get; set; }
    }
}