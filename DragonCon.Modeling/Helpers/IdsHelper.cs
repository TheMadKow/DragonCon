using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Helpers
{
    public static class IdsHelper
    {
        public static string GetCollectionFromId(this string ravenIdent)
        {
            try
            {
                return ravenIdent.Split('/')[0];
            }
            catch
            {
                return ravenIdent;
            }
        }
        public static string GetNumberFromId(this string ravenIdent)
        {
            try
            {
                return ravenIdent.Split('/')[1];
            }
            catch
            {
                return ravenIdent;
            }
        }

        public static string FixRavenId(this string ravenIdent, string collection)
        {
            if (ravenIdent.ToLower().StartsWith(collection.ToLower()) == false)
                ravenIdent = $"{collection}/{ravenIdent}";

            return ravenIdent;

        }

    }
}
