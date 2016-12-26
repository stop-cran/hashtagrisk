using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskApp.Calculations
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipEach<T>(this IEnumerable<T> items, int skip)
        {
            int i = skip;
            foreach (var item in items)
                if (i == skip)
                {
                    i = 1;
                    yield return item;
                }
                else
                    i++;
        }

        public static IEnumerable<T> Fix<T>(T seed, Func<T, T> transform)
        {
            for (; ; seed = transform(seed))
                yield return seed;
        }

        public static IEnumerable<TAccumulate> Scan<TSource, TAccumulate>(this IEnumerable<TSource> items, Func<TAccumulate, TSource, TAccumulate> transform)
        {
            var seed = default(TAccumulate);

            foreach (var item in items)
            {
                seed = transform(seed, item);
                yield return seed;
            }
        }
    }
}