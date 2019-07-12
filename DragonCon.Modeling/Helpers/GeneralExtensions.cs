using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonCon.Modeling.Helpers
{
    public static class GeneralExtensions
    {
        public static bool IsEmptyString(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotEmptyString(this string s) => IsEmptyString(s) == false;

        public static bool IsAlphaNumeric(this string s) => s.All(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x));

        public static bool IsNotAlphaNumeric(this string s) => !s.IsAlphaNumeric();


        public static bool Missing<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return source.Contains(value) == false;
        }

        public static bool MissingKey<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
        {
            return dictionary.ContainsKey(key) == false;
        }

        public static bool ContainsSubset<T>(this IEnumerable<T> set, IEnumerable<T> subSet)
        {
            return !subSet.Except(set).Any();
        }

        public static bool ContainsAny(this string src, IEnumerable<string> tokens)
        {
            var lSrc = src?.ToLower() ?? string.Empty;
            return lSrc.IsNotEmptyString() && tokens.Any(lSrc.Contains);
        }

        public static IList<T> AddRange<T>(this IList<T> src, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                src.Add(item);
            }

            return src;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
