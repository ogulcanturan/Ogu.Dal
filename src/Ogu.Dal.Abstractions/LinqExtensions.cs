using System;
using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.Abstractions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> LongSkip<T>(this IQueryable<T> items, long count) => LongSkip(items, int.MaxValue, count);
        internal static IQueryable<T> LongSkip<T>(this IQueryable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0; i < segmentCount; i++)
                items = items.Skip(maxSize);

            if (remainder != 0)
                items = items.Skip((int)remainder);

            return items;
        }

        public static IQueryable<T> LongTake<T>(this IQueryable<T> items, long count) => LongTake(items, int.MaxValue, count);
        internal static IQueryable<T> LongTake<T>(this IQueryable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0; i < segmentCount; i++)
                items = items.Take(maxSize);

            if (remainder != 0)
                items = items.Take((int)remainder);

            return items;
        }

        public static IEnumerable<T> LongSkip<T>(this IEnumerable<T> items, long count) => LongSkip(items, int.MaxValue, count);
        internal static IEnumerable<T> LongSkip<T>(this IEnumerable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                items = items.Skip(maxSize);

            if (remainder != 0)
                items = items.Skip((int)remainder);

            return items;
        }

        public static IEnumerable<T> LongTake<T>(this IEnumerable<T> items, long count) => LongTake(items, int.MaxValue, count);
        internal static IEnumerable<T> LongTake<T>(this IEnumerable<T> items, int maxSize, long count)
        {
            var segmentCount = Math.DivRem(count, maxSize, out var remainder);

            for (var i = 0L; i < segmentCount; i++)
                items = items.Take(maxSize);

            if (remainder != 0)
                items = items.Take((int)remainder);

            return items;
        }

#if !NET6_0_OR_GREATER

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();

            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

#endif
    }
}