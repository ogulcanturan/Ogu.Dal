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

        public Paginated(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages, TModel[] entities)
        {
            Items = entities ?? Array.Empty<TModel>();

            PagingInfo = new x86.PagingInfo(pageIndex, itemsPerPage, totalItems, rangeOfPages);
        }

        public Paginated(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages, IList<TModel> entities)
        {
            Items = entities ?? new List<TModel>();

            PagingInfo = new x86.PagingInfo(pageIndex, itemsPerPage, totalItems, rangeOfPages);
        }

        public Paginated(int totalItems, TModel[] entities)
        {
            Items = entities ?? Array.Empty<TModel>();

            PagingInfo = new x86.PagingInfo(1, totalItems, totalItems, 0);
        }

        public Paginated(int totalItems, IList<TModel> entities)
        {
            Items = entities ?? new List<TModel>();

            PagingInfo = new x86.PagingInfo(1, totalItems, totalItems, 0);
        }

        public IPagingInfo<int> PagingInfo { get; }
        public IEnumerable<TModel> Items { get; }
    }

    public class PaginatedDto<TDto> : IPaginated<TDto>
    {
        public PaginatedDto() { }
        public PaginatedDto(IPagingInfo<int> pagingInfo, IEnumerable<TDto> items)
        {
            PagingInfo = new x86.PagingInfoDto
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

        public IPagingInfo<int> PagingInfo { get; set; }
        public IEnumerable<TDto> Items { get; set; }
    }
}