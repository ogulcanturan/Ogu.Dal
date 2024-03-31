using System;
using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.Abstractions
{
    public static class PaginatedExtensions
    {
        public static IPaginated<TEntity> ToPaginated<TEntity>(this TEntity[] items, int pageIndex, int itemsPerPage = 0, int rangeOfPages = 0)
        {
            var totalItems = 0;

            if (items == null)
                return new Paginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, Array.Empty<TEntity>());

            totalItems = items.Length;

            if (totalItems > 0 && itemsPerPage > 0 && pageIndex > 0)
            {
                items = items.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
            }

            return new Paginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, items);
        }

        public static IPaginated<TEntity> ToPaginated<TEntity>(this TEntity[] items)
        {
            return items != null ? new Paginated<TEntity>(items.Length, items) : new Paginated<TEntity>(0, Array.Empty<TEntity>());
        }

        public static ILongPaginated<TEntity> ToLongPaginated<TEntity>(this TEntity[] items, long pageIndex, long itemsPerPage = 0, long rangeOfPages = 0)
        {
            var totalItems = 0L;

            if (items == null)
                return new LongPaginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, Array.Empty<TEntity>());

            totalItems = items.LongLength;

            if (totalItems > 0 && itemsPerPage > 0
                               && pageIndex > 0)
            {
                items = items.LongSkip((pageIndex - 1) * itemsPerPage).LongTake(itemsPerPage).ToArray();
            }
            return new LongPaginated<TEntity>(pageIndex, itemsPerPage, totalItems, rangeOfPages, items);
        }

        public static ILongPaginated<TEntity> ToLongPaginated<TEntity>(this TEntity[] items)
        {
            return items != null ? new LongPaginated<TEntity>(items.LongLength, items) : new LongPaginated<TEntity>(0, Array.Empty<TEntity>());
        }

        public static IPaginated<TDto> ToPaginatedDto<TDto, TEntity>(this IPaginated<TEntity> paginated, IEnumerable<TDto> items) where TEntity : class
        {
            return new PaginatedDto<TDto>(paginated.PagingInfo, items);
        }

        public static ILongPaginated<TDto> ToLongPaginatedDto<TDto, TEntity>(this ILongPaginated<TEntity> paginated, IEnumerable<TDto> items) where TEntity : class
        {
            return new LongPaginatedDto<TDto>(paginated.PagingInfo, items);
        }
    }
}