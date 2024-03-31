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

        public LongPaginated(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages, TModel[] entities)
        {
            Items = entities ?? Array.Empty<TModel>();

            PagingInfo = new x64.PagingInfo(pageIndex, itemsPerPage, totalItems, rangeOfPages);
        }

        public LongPaginated(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages, IList<TModel> entities)
        {
            Items = entities ?? new List<TModel>();

            PagingInfo = new x64.PagingInfo(pageIndex, itemsPerPage, totalItems, rangeOfPages);
        }

        public LongPaginated(long totalItems, TModel[] entities)
        {
            Items = entities ?? Array.Empty<TModel>();

            PagingInfo = new x64.PagingInfo(1, totalItems, totalItems, 0);
        }

        public LongPaginated(long totalItems, IList<TModel> entities)
        {
            Items = entities ?? new List<TModel>();

            PagingInfo = new x64.PagingInfo(1, totalItems, totalItems, 0);
        }

        public IPagingInfo<long> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; set; }
    }

    public class LongPaginatedDto<TDto> : ILongPaginated<TDto>
    {
        public LongPaginatedDto() { }
        public LongPaginatedDto(IPagingInfo<long> pagingInfo, IEnumerable<TDto> items)
        {
            PagingInfo = new x64.PagingInfoDto
            {
                PageIndexItems = pagingInfo.PageIndexItems,
                PageIndex = pagingInfo.PageIndex,
                ItemsPerPage = pagingInfo.ItemsPerPage,
                TotalItems = pagingInfo.TotalItems,
                TotalPages = pagingInfo.TotalPages,
                HasNextPage = pagingInfo.HasNextPage,
                HasPreviousPage = pagingInfo.HasPreviousPage,
                RangeOfPages = pagingInfo.RangeOfPages,
                FinishIndex = pagingInfo.FinishIndex,
                StartIndex = pagingInfo.StartIndex,
            };
            Items = items;
        }

        public IPagingInfo<long> PagingInfo { get; set; }
        public IEnumerable<TDto> Items { get; set; }
    }
}