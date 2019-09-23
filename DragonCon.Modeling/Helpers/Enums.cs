using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DragonCon.Modeling.Helpers
{
 public static class Enums
    {
        private static string GetDescription(this Enum enumValue, string defDesc)
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());
            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes
                        (typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }
     
        private static string GetDescription(this Enum enumValue)
        {
            return GetDescription(enumValue, null);
        }

        public static int AsInt(this Enum e)
        {
            return Convert.ToInt32(e);
        }

        private static T EnumFromDescription<T>(this string description)
        {
            Type t = typeof(T);
            foreach (FieldInfo fi in t.GetFields())
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    foreach (DescriptionAttribute attr in attrs)
                    {
                        if (attr.Description.Equals(description))
                            return (T)fi.GetValue(null);
                    }
                }
            }
            return default(T);
        }

        private static bool HasDescription<T>(this T k)
        {
            Type t = typeof(T);
            foreach (FieldInfo fi in t.GetFields())
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<T> GetValues<T>()
        {
            var array = Enum.GetValues(typeof(T));
            return array.Cast<T>().ToList();
        }

        public static string ToTextual(this Enum input)
        {
            var desc = input.GetDescription();
            return string.IsNullOrEmpty(desc) ? input.ToString() : desc;
        }

        public static T EnumFromTextual<T>(this string input)
        {
            if (input == null)
                return default(T);

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var t = default(T);
            if (t.HasDescription())
            {
                return input.EnumFromDescription<T>();
            }
            else
            {
                return (T)Enum.Parse(t.GetType(), input);
            }
        }

        public static IEnumerable<SelectListItem> AsSelectListItem(this Enum input)
        {
            var type = input.GetType();
            var vals = Enum.GetValues(type);

            var results = (from Enum value in vals
                select new SelectListItem()
                {
                    Value = value.ToString(),
                    Text = value.ToTextual()
                });

            return results;
        }

        public static IEnumerable<SelectListItem> AsSelectListItem<T>()
        where T : Enum
        {
            var type = typeof(T);
            var vals = Enum.GetValues(type);

            var results = (from Enum value in vals
                select new SelectListItem()
                {
                    Value = value.ToString(),
                    Text = value.ToTextual()
                });

            return results;
        }


    }}
