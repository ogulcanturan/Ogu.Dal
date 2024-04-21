using System;
using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.Abstractions
{
    public static class PaginatedExtensions
    {
        public static IPaginated<TModel> ToPaginated<TModel>(this IEnumerable<TModel> items, int pageIndex, int itemsPerPage, int rangeOfPages = 0)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var list = items as IList<TModel> ?? items.ToArray();

            return new Paginated<TModel>(pageIndex, itemsPerPage, list.Count, rangeOfPages,
                list.Count > 0 && itemsPerPage > 0 && pageIndex > 0
                    ? list.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage)
                    : list);
        }

        public static IPaginated<TModel> ToPaginated<TModel>(this IEnumerable<TModel> items, int totalItems, int pageIndex, int itemsPerPage, int rangeOfPages)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new Paginated<TModel>(pageIndex, itemsPerPage, totalItems, rangeOfPages, items);
        }

        public static IPaginated<TModel> ToPaginated<TModel>(this IEnumerable<TModel> items, int totalItems)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new Paginated<TModel>(totalItems, items);
        }

        public static IPaginated<TModel> ToPaginated<TModel>(this IEnumerable<TModel> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new Paginated<TModel>(items);
        }

        public static ILongPaginated<TModel> ToLongPaginated<TModel>(this IEnumerable<TModel> items, long pageIndex, long itemsPerPage, long rangeOfPages = 0)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var list = items as TModel[] ?? items.ToArray();

            return new LongPaginated<TModel>(pageIndex, itemsPerPage, list.LongLength, rangeOfPages,
                list.LongLength > 0 && itemsPerPage > 0 && pageIndex > 0
                    ? list.LongSkip((pageIndex - 1) * itemsPerPage).LongTake(itemsPerPage)
                    : list);
        }

        public static ILongPaginated<TModel> ToLongPaginated<TModel>(this IEnumerable<TModel> items, long totalItems, long pageIndex, long itemsPerPage, long rangeOfPages)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new LongPaginated<TModel>(pageIndex, itemsPerPage, totalItems, rangeOfPages, items);
        }

        public static ILongPaginated<TModel> ToLongPaginated<TModel>(this IEnumerable<TModel> items, long totalItems)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new LongPaginated<TModel>(totalItems, items);
        }

        public static ILongPaginated<TModel> ToLongPaginated<TModel>(this IEnumerable<TModel> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return new LongPaginated<TModel>(items);
        }

        public static IPaginated<TDto> ToPaginatedDto<TDto, TModel>(this IPaginated<TModel> paginated, IEnumerable<TDto> items) where TModel : class
        {
            return new PaginatedDto<TDto>(paginated.PagingInfo, items);
        }

        public static IPaginated<TDto> ToPaginatedDto<TDto, TModel>(this IPaginated<TModel> paginated, Func<IEnumerable<TModel>, IEnumerable<TDto>> func) where TModel : class
        {
            return ToPaginatedDto(paginated, func(paginated.Items));
        }

        public static ILongPaginated<TDto> ToLongPaginatedDto<TDto, TModel>(this ILongPaginated<TModel> paginated, IEnumerable<TDto> items) where TModel : class
        {
            return new LongPaginatedDto<TDto>(paginated.PagingInfo, items);
        }

        public static ILongPaginated<TDto> ToLongPaginatedDto<TDto, TModel>(this ILongPaginated<TModel> paginated, Func<IEnumerable<TModel>, IEnumerable<TDto>> func) where TModel : class
        {
            return ToLongPaginatedDto(paginated, func(paginated.Items));
        }
    }
}