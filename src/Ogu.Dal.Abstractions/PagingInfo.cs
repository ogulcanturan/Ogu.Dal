using System;

namespace Ogu.Dal.Abstractions
{
    public interface IPagingInfo<TType>
    {
        TType PageIndex { get; set; }
        TType ItemsPerPage { get; set; }
        TType TotalItems { get; set; }
        TType TotalPages { get; }
        TType PageIndexItems { get; }
        TType RangeOfPages { get; }
        TType StartIndex { get; }
        TType FinishIndex { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }

    namespace x86
    {
        public struct PagingInfo : IPagingInfo<int>
        {
            public PagingInfo(int pageIndex, int itemsPerPage, int totalItems, int rangeOfPages)
            {
                PageIndex = pageIndex;
                ItemsPerPage = itemsPerPage;
                TotalItems = totalItems;
                RangeOfPages = rangeOfPages;
            }

            public int PageIndex { get; set; }
            public int ItemsPerPage { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages => ItemsPerPage > 0 ? (int)Math.Ceiling((double)TotalItems / ItemsPerPage) : 1;
            public int RangeOfPages { get; set; }
            public int StartIndex => Math.Max(PageIndex - RangeOfPages, 1);
            public int FinishIndex => Math.Min(PageIndex + RangeOfPages, TotalPages);
            public bool HasNextPage => PageIndex < TotalPages;
            public bool HasPreviousPage => PageIndex > 1;
            public int PageIndexItems
            {
                get
                {
                    if (ItemsPerPage < 1 || PageIndex < 1)
                    {
                        PageIndex = 1;
                        return ItemsPerPage = TotalItems;
                    }

                    if (HasNextPage)
                        return ItemsPerPage;

                    var remainder = TotalItems % ItemsPerPage;
                    return remainder == 0 && TotalItems > 0 ? ItemsPerPage : remainder;
                }
            }
        }

        public struct PagingInfoDto : IPagingInfo<int>
        {
            public int PageIndex { get; set; }
            public int ItemsPerPage { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
            public int PageIndexItems { get; set; }
            public int RangeOfPages { get; set; }
            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
        }
    }

    namespace x64
    {
        public struct PagingInfo : IPagingInfo<long>
        {
            public PagingInfo(long pageIndex, long itemsPerPage, long totalItems, long rangeOfPages)
            {
                PageIndex = pageIndex;
                ItemsPerPage = itemsPerPage;
                TotalItems = totalItems;
                RangeOfPages = rangeOfPages;
            }

            public long PageIndex { get; set; }
            public long ItemsPerPage { get; set; }
            public long TotalItems { get; set; }
            public long TotalPages => ItemsPerPage > 0 ? (long)Math.Ceiling((decimal)TotalItems / ItemsPerPage) : 1;
            public long RangeOfPages { get; set; }
            public long StartIndex => Math.Max(PageIndex - RangeOfPages, 1);
            public long FinishIndex => Math.Min(PageIndex + RangeOfPages, TotalPages);
            public bool HasNextPage => PageIndex < TotalPages;
            public bool HasPreviousPage => PageIndex > 1;
            public long PageIndexItems
            {
                get
                {
                    if (ItemsPerPage < 1 || PageIndex < 1)
                    {
                        PageIndex = 1;
                        return ItemsPerPage = TotalItems;
                    }

                    if (HasNextPage)
                        return ItemsPerPage;

                    var remainder = TotalItems % ItemsPerPage;
                    return remainder == 0 && TotalItems > 0 ? ItemsPerPage : remainder;
                }
            }
        }

        public struct PagingInfoDto : IPagingInfo<long>
        {
            public long PageIndex { get; set; }
            public long ItemsPerPage { get; set; }
            public long TotalItems { get; set; }
            public long TotalPages { get; set; }
            public long PageIndexItems { get; set; }
            public long RangeOfPages { get; set; }
            public long StartIndex { get; set; }
            public long FinishIndex { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
        }
    }
}