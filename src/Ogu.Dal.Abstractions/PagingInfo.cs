using System;

namespace Ogu.Dal.Abstractions
{
    public interface IPagingInfo<out TType>
    {
        TType PageIndex { get; }
        TType PageIndexItemsCount { get; }
        TType ItemsPerPage { get; }
        TType TotalItemsCount { get; }
        TType TotalPagesCount { get; }
        TType RangeOfPages { get; }
        TType StartIndex { get; }
        TType FinishIndex { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }

    namespace x86
    {
        public readonly struct PagingInfo : IPagingInfo<int>
        {
            public PagingInfo(int pageIndex, int pageIndexItemsCount, int itemsPerPage, int totalItemsCount, int rangeOfPages)
            {
                PageIndex = pageIndex;
                PageIndexItemsCount = pageIndexItemsCount;
                ItemsPerPage = itemsPerPage;
                TotalItemsCount = totalItemsCount;
                RangeOfPages = rangeOfPages;

                if (itemsPerPage < 1 || PageIndex < 1)
                {
                    PageIndex = 1;

                    if(TotalItemsCount > 0)
                        ItemsPerPage = TotalItemsCount;
                }

                TotalPagesCount = TotalItemsCount > 0 ? (ItemsPerPage > 0 ? (int)Math.Ceiling((double)TotalItemsCount / ItemsPerPage) : 1) : 0;

                StartIndex = Math.Max(PageIndex - RangeOfPages, 1);
                FinishIndex = Math.Min(PageIndex + RangeOfPages, TotalPagesCount);
                HasNextPage = PageIndex < TotalPagesCount;
                HasPreviousPage = PageIndex > 1;
            }

            public int PageIndex { get; }
            public int PageIndexItemsCount { get; }
            public int ItemsPerPage { get; }
            public int TotalItemsCount { get; }
            public int TotalPagesCount { get; }
            public int RangeOfPages { get; }
            public int StartIndex { get; }
            public int FinishIndex { get; }
            public bool HasNextPage { get; }
            public bool HasPreviousPage { get; }
        }

        public struct PagingInfoDto : IPagingInfo<int>
        {
            public int PageIndex { get; set; }
            public int PageIndexItemsCount { get; set; }
            public int ItemsPerPage { get; set; }
            public int TotalItemsCount { get; set; }
            public int TotalPagesCount { get; set; }
            public int RangeOfPages { get; set; }
            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
        }
    }

    namespace x64
    {
        public readonly struct PagingInfo : IPagingInfo<long>
        {
               public PagingInfo(long pageIndex, long pageIndexItemsCount, long itemsPerPage, long totalItemsCount, long rangeOfPages)
            {
                PageIndex = pageIndex;
                PageIndexItemsCount = pageIndexItemsCount;
                ItemsPerPage = itemsPerPage;
                TotalItemsCount = totalItemsCount;
                RangeOfPages = rangeOfPages;

                if (itemsPerPage < 1 || PageIndex < 1)
                {
                    PageIndex = 1;

                    if(TotalItemsCount > 0)
                        ItemsPerPage = TotalItemsCount;
                }

                TotalPagesCount = TotalItemsCount > 0 ? (ItemsPerPage > 0 ? (long)Math.Ceiling((decimal)TotalItemsCount / ItemsPerPage) : 1) : 0;

                StartIndex = Math.Max(PageIndex - RangeOfPages, 1);
                FinishIndex = Math.Min(PageIndex + RangeOfPages, TotalPagesCount);
                HasNextPage = PageIndex < TotalPagesCount;
                HasPreviousPage = PageIndex > 1;
            }

            public long PageIndex { get; }
            public long PageIndexItemsCount { get; }
            public long ItemsPerPage { get; }
            public long TotalItemsCount { get; }
            public long TotalPagesCount { get; }
            public long RangeOfPages { get; }
            public long StartIndex { get; }
            public long FinishIndex { get; }
            public bool HasNextPage { get; }
            public bool HasPreviousPage { get; }
        }

        public struct PagingInfoDto : IPagingInfo<long>
        {
            public long PageIndex { get; set; }
            public long PageIndexItemsCount { get; set; }
            public long ItemsPerPage { get; set; }
            public long TotalItemsCount { get; set; }
            public long TotalPagesCount { get; set; }
            public long RangeOfPages { get; set; }
            public long StartIndex { get; set; }
            public long FinishIndex { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
        }
    }
}