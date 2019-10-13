using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace DragonCon.Modeling.Helpers
{
    public static class GeneralExtensions
    {
        public static string AsYesNoHebrew(this bool toggle)
        {
            return toggle ? "כן" : "לא";
        }
        public static string InHebrew(this IsoDayOfWeek dow)
        {
            return dow switch
            {
                IsoDayOfWeek.None       => "",
                IsoDayOfWeek.Sunday     => "ראשון",
                IsoDayOfWeek.Monday     => "שני",
                IsoDayOfWeek.Tuesday    => "שלישי",
                IsoDayOfWeek.Wednesday  => "רביעי",
                IsoDayOfWeek.Thursday   => "חמישי",
                IsoDayOfWeek.Friday     => "שישי",
                IsoDayOfWeek.Saturday   => "שבת",
                _                       => throw new ArgumentOutOfRangeException(nameof(dow), dow, null)
            };
        }

        public static (string Major, string Minor) SplitTuples(this string input)
        {
            var result = input.Split(new[] {','}, StringSplitOptions.None);
            return (result[0], result[1]);
        }


        public static bool IsEmptyString(this string? s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotEmptyString(this string? s) => IsEmptyString(s) == false;

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

        private static JsonSerializerSettings BuildDefaultJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.CurrentCulture,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };
            settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            return settings;
        }

        public static string? AsJson(this object input, bool indented = false)
        {
            if (input is null)
                return null;

            var defaults = BuildDefaultJsonSettings();
            defaults.Formatting = (indented) ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(input, defaults);
        }

        public static T ObjectFromJson<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input, BuildDefaultJsonSettings());
        }


    }
}
